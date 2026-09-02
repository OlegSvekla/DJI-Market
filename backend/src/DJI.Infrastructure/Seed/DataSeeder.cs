using DJI.Core.Entities;
using DJI.Core.Enums;
using DJI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DJI.Infrastructure.Seed;

public class DataSeeder(
    DjiDbContext context,
    IOptions<SeedOptions> options,
    ILogger<DataSeeder> logger)
{
    private const int BatchSize = 500;

    private static readonly double[] MonthlyFactors =
        [0.78, 0.86, 1.02, 1.06, 1.12, 0.98, 0.82, 0.92, 1.14, 1.20, 1.34, 1.42];

    private readonly SeedOptions _options = options.Value;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await context.Sales.AnyAsync(ct))
        {
            return;
        }

        var random = new Random(_options.RandomSeed);
        var anchor = _options.AnchorDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var start = anchor.AddMonths(-_options.MonthsOfHistory).AddDays(1);

        var products = await SeedProductsAsync(ct);
        var managers = await SeedManagersAsync(random, anchor, ct);
        var customers = await SeedCustomersAsync(random, ct);

        var salesCount = await SeedSalesAsync(random, start, anchor, managers, customers, products, ct);

        logger.LogInformation(
            "Seed completed: {Sales} sales, {Managers} managers, {Customers} customers, {Products} products.",
            salesCount,
            managers.Count,
            customers.Count,
            products.Count);
    }

    private async Task<List<ProductWeight>> SeedProductsAsync(CancellationToken ct)
    {
        var weights = new List<ProductWeight>();

        foreach (var categorySpec in SeedCatalog.Categories)
        {
            var category = new Category { Name = categorySpec.Name };

            context.Categories.Add(category);

            foreach (var productSpec in categorySpec.Products)
            {
                var product = new Product
                {
                    Name = productSpec.Name,
                    Sku = productSpec.Sku,
                    Category = category,
                    ListPrice = productSpec.ListPrice,
                    BaseCost = Math.Round(productSpec.ListPrice * (1m - productSpec.Margin), 2),
                };

                context.Products.Add(product);

                var popularity = productSpec.ListPrice switch
                {
                    < 10_000m => 3.4,
                    < 40_000m => 2.6,
                    < 100_000m => 1.6,
                    < 400_000m => 0.8,
                    _ => 0.25,
                };

                weights.Add(new ProductWeight(product, popularity));
            }
        }

        await context.SaveChangesAsync(ct);

        return weights;
    }

    private async Task<List<ManagerProfile>> SeedManagersAsync(
        Random random,
        DateOnly anchor,
        CancellationToken ct)
    {
        var profiles = new List<ManagerProfile>(_options.Managers);

        for (var index = 0; index < _options.Managers; index++)
        {
            var firstNameIndex = index % SeedCatalog.FirstNames.Length;

            var manager = new Manager
            {
                FirstName = SeedCatalog.FirstNames[firstNameIndex],
                LastName = SeedCatalog.LastNameFor(firstNameIndex, index),
                Team = SeedCatalog.Teams[index % SeedCatalog.Teams.Length],
                Position = SeedCatalog.Positions[index % SeedCatalog.Positions.Length],
                AvatarColor = SeedCatalog.AvatarColors[index % SeedCatalog.AvatarColors.Length],
                IsActive = index != _options.Managers - 1,
                HiredOn = anchor.AddDays(-random.Next(400, 2200)),
            };

            var volume = index switch
            {
                0 => 2.4,
                1 => 2.0,
                2 => 1.7,
                < 8 => 1.25 - (index - 3) * 0.06,
                < 15 => 0.95 - (index - 8) * 0.07,
                _ => 0.30 + random.NextDouble() * 0.20,
            };

            var checkBias = 0.55 + random.NextDouble() * 1.30;

            var discountBias = 0.02 + random.NextDouble() * 0.12;

            var troubleBias = volume < 0.7 ? 1.8 : 1.0;

            DateOnly? gapFrom = null;
            DateOnly? gapTo = null;

            if (index % 5 == 3)
            {
                var gapStart = anchor.AddDays(-random.Next(60, 300));

                gapFrom = gapStart;
                gapTo = gapStart.AddDays(random.Next(35, 70));
            }

            var activeFrom = index == _options.Managers - 2
                ? anchor.AddMonths(-4)
                : DateOnly.MinValue;

            var activeTo = index == _options.Managers - 1
                ? anchor.AddDays(-45)
                : DateOnly.MaxValue;

            profiles.Add(new ManagerProfile(
                manager,
                volume,
                checkBias,
                discountBias,
                troubleBias,
                activeFrom,
                activeTo,
                gapFrom,
                gapTo));

            context.Managers.Add(manager);
        }

        await context.SaveChangesAsync(ct);

        return profiles;
    }

    private async Task<List<Customer>> SeedCustomersAsync(Random random, CancellationToken ct)
    {
        var customers = new List<Customer>(_options.Customers);
        var usedCompanies = new HashSet<string>();

        for (var index = 0; index < _options.Customers; index++)
        {
            string company;
            var attempt = 0;

            do
            {
                var form = SeedCatalog.CompanyForms[random.Next(SeedCatalog.CompanyForms.Length)];
                var prefix = SeedCatalog.CompanyPrefixes[random.Next(SeedCatalog.CompanyPrefixes.Length)];
                var suffix = SeedCatalog.CompanySuffixes[random.Next(SeedCatalog.CompanySuffixes.Length)];

                company = $"{form} {prefix}{suffix.ToLowerInvariant()}";

                if (++attempt > 50)
                {
                    company = $"{company} {index}";
                }
            }
            while (!usedCompanies.Add(company));

            var contactNameIndex = random.Next(SeedCatalog.FirstNames.Length);

            var customer = new Customer
            {
                Name = SeedCatalog.FirstNames[contactNameIndex]
                    + " "
                    + SeedCatalog.LastNameFor(contactNameIndex, random.Next(1000)),
                Company = company,
                Segment = index switch
                {
                    < 12 => CustomerSegmentEnum.Enterprise,
                    < 36 => CustomerSegmentEnum.MidMarket,
                    _ => CustomerSegmentEnum.Smb,
                },
            };

            customers.Add(customer);
            context.Customers.Add(customer);
        }

        await context.SaveChangesAsync(ct);

        return customers;
    }

    private async Task<int> SeedSalesAsync(
        Random random,
        DateOnly start,
        DateOnly anchor,
        List<ManagerProfile> managers,
        List<Customer> customers,
        List<ProductWeight> products,
        CancellationToken ct)
    {
        var days = BuildDayWeights(start, anchor);
        var productPicker = new WeightedPicker(products.Select(product => product.Popularity));
        var batch = new List<Sale>(BatchSize);
        var created = 0;

        for (var index = 0; index < _options.Sales; index++)
        {
            var date = days.Pick(random);
            var manager = PickManager(random, managers, date);

            if (manager is null)
            {
                continue;
            }

            var customer = PickCustomer(random, customers);
            var sale = BuildSale(random, date, manager, customer, products, productPicker, index);

            batch.Add(sale);
            created++;

            if (batch.Count < BatchSize)
            {
                continue;
            }

            await FlushAsync(batch, ct);
        }

        batch.Add(BuildWhaleSale(random, anchor, managers, customers, products));
        created++;

        await FlushAsync(batch, ct);

        return created;
    }

    private async Task FlushAsync(List<Sale> batch, CancellationToken ct)
    {
        if (batch.Count == 0)
        {
            return;
        }

        context.Sales.AddRange(batch);

        await context.SaveChangesAsync(ct);

        context.ChangeTracker.Clear();
        batch.Clear();
    }

    private Sale BuildSale(
        Random random,
        DateOnly date,
        ManagerProfile manager,
        Customer customer,
        List<ProductWeight> products,
        WeightedPicker productPicker,
        int index)
    {
        var sale = new Sale
        {
            Number = $"SO-{date:yyyyMM}-{index + 1:0000}",
            ManagerId = manager.Entity.Id,
            CustomerId = customer.Id,
            SaleDate = date,
            Status = PickStatus(random, manager),
            Items = [],
        };

        var itemCount = PickItemCount(random, customer.Segment, manager.CheckBias);

        for (var i = 0; i < itemCount; i++)
        {
            var product = products[productPicker.Pick(random)].Product;

            sale.Items.Add(BuildItem(random, product, customer.Segment, manager));
        }

        return sale;
    }

    private static SaleItem BuildItem(
        Random random,
        Product product,
        CustomerSegmentEnum segment,
        ManagerProfile manager)
    {
        var quantity = segment switch
        {
            CustomerSegmentEnum.Enterprise => random.Next(1, product.ListPrice > 300_000m ? 3 : 8),
            CustomerSegmentEnum.MidMarket => random.Next(1, product.ListPrice > 300_000m ? 2 : 5),
            _ => random.Next(1, 3),
        };

        var discount = manager.DiscountBias + random.NextDouble() * 0.06;

        if (segment == CustomerSegmentEnum.Enterprise)
        {
            discount += 0.04;
        }

        var productMargin = (double)(1m - product.BaseCost / product.ListPrice);
        var maxDiscount = Math.Max(0.02, productMargin * 0.45);

        discount = Math.Min(discount, maxDiscount);

        if (random.NextDouble() < 0.03)
        {
            discount = productMargin + 0.03;
        }

        var unitPrice = Math.Round(product.ListPrice * (decimal)(1 - discount), 2);

        var unitCost = Math.Round(product.BaseCost * (decimal)(0.96 + random.NextDouble() * 0.08), 2);

        return new SaleItem
        {
            ProductId = product.Id,
            Quantity = Math.Max(1, quantity),
            UnitPrice = unitPrice,
            UnitCost = unitCost,
        };
    }

    private Sale BuildWhaleSale(
        Random random,
        DateOnly anchor,
        List<ManagerProfile> managers,
        List<Customer> customers,
        List<ProductWeight> products)
    {
        var manager = managers[0];
        var customer = customers[0];
        var flagship = products.OrderByDescending(product => product.Product.ListPrice).First().Product;

        var sale = new Sale
        {
            Number = $"SO-{anchor:yyyyMM}-9999",
            ManagerId = manager.Entity.Id,
            CustomerId = customer.Id,
            SaleDate = anchor.AddDays(-random.Next(3, 20)),
            Status = SaleStatusEnum.Paid,
            Items =
            [
                new SaleItem
                {
                    ProductId = flagship.Id,
                    Quantity = 6,
                    UnitPrice = Math.Round(flagship.ListPrice * 0.94m, 2),
                    UnitCost = flagship.BaseCost,
                },
            ],
        };

        return sale;
    }

    private static SaleStatusEnum PickStatus(Random random, ManagerProfile manager)
    {
        var roll = random.NextDouble();
        var cancelled = 0.06 * manager.TroubleBias;
        var refunded = 0.045 * manager.TroubleBias;

        if (roll < cancelled)
        {
            return SaleStatusEnum.Cancelled;
        }

        return roll < cancelled + refunded
            ? SaleStatusEnum.Refunded
            : SaleStatusEnum.Paid;
    }

    private static int PickItemCount(Random random, CustomerSegmentEnum segment, double checkBias)
    {
        var roll = random.NextDouble() * checkBias;

        var baseCount = roll switch
        {
            < 0.45 => 1,
            < 0.75 => 2,
            < 0.92 => 3,
            _ => 5,
        };

        return segment == CustomerSegmentEnum.Enterprise ? baseCount + 1 : baseCount;
    }

    private static Customer PickCustomer(Random random, List<Customer> customers)
    {
        return random.NextDouble() < 0.33
            ? customers[random.Next(Math.Min(15, customers.Count))]
            : customers[random.Next(customers.Count)];
    }

    private static ManagerProfile? PickManager(Random random, List<ManagerProfile> managers, DateOnly date)
    {
        var available = managers.Where(manager => manager.WorksOn(date)).ToList();

        if (available.Count == 0)
        {
            return null;
        }

        var picker = new WeightedPicker(available.Select(manager => manager.Volume));

        return available[picker.Pick(random)];
    }

    private static DayPicker BuildDayWeights(DateOnly start, DateOnly anchor)
    {
        var dates = new List<DateOnly>();
        var weights = new List<double>();

        for (var date = start; date <= anchor; date = date.AddDays(1))
        {
            var weekday = date.DayOfWeek switch
            {
                DayOfWeek.Saturday => 0.35,
                DayOfWeek.Sunday => 0.18,
                DayOfWeek.Monday => 1.10,
                _ => 1.0,
            };

            dates.Add(date);
            weights.Add(MonthlyFactors[date.Month - 1] * weekday);
        }

        return new DayPicker(dates, weights);
    }

    private sealed record ProductWeight(Product Product, double Popularity);

    private sealed record ManagerProfile(
        Manager Entity,
        double Volume,
        double CheckBias,
        double DiscountBias,
        double TroubleBias,
        DateOnly ActiveFrom,
        DateOnly ActiveTo,
        DateOnly? GapFrom,
        DateOnly? GapTo)
    {
        public bool WorksOn(DateOnly date)
        {
            if (date < ActiveFrom || date > ActiveTo)
            {
                return false;
            }

            return GapFrom is null || GapTo is null || date < GapFrom || date > GapTo;
        }
    }

    private sealed class WeightedPicker
    {
        private readonly double[] _cumulative;

        public WeightedPicker(IEnumerable<double> weights)
        {
            var running = 0.0;

            _cumulative = weights
                .Select(weight => running += weight)
                .ToArray();
        }

        public int Pick(Random random)
        {
            var target = random.NextDouble() * _cumulative[^1];
            var index = Array.BinarySearch(_cumulative, target);

            if (index < 0)
            {
                index = ~index;
            }

            return Math.Min(index, _cumulative.Length - 1);
        }
    }

    private sealed class DayPicker(List<DateOnly> dates, List<double> weights)
    {
        private readonly WeightedPicker _picker = new(weights);

        public DateOnly Pick(Random random) => dates[_picker.Pick(random)];
    }
}
