using DJI.Core.Entities;
using DJI.Core.Enums;
using DJI.Infrastructure.Persistence;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Tests.Analytics;

public sealed class AnalyticsTestContext : IDisposable
{
    private static int _databaseNumber;

    private readonly DjiDbContext _context;

    public AnalyticsTestContext()
    {
        var options = new DbContextOptionsBuilder<DjiDbContext>()
            .UseInMemoryDatabase($"dji-tests-{Interlocked.Increment(ref _databaseNumber)}")
            .Options;

        _context = new DjiDbContext(options);

        var category = new Category { Id = 1, Name = "Дроны" };

        Product = new Product
        {
            Id = 1,
            Name = "Mavic 4 Pro",
            Sku = "DRN-M4P",
            CategoryId = category.Id,
            Category = category,
            ListPrice = 1_000m,
            BaseCost = 600m,
        };

        _context.Categories.Add(category);
        _context.Products.Add(Product);

        _context.Managers.AddRange(
            NewManager(1, "Алексей", "Ковалёв"),
            NewManager(2, "Мария", "Смирнова"));

        _context.Customers.Add(new Customer
        {
            Id = 1,
            Name = "Игорь Орлов",
            Company = "ООО Аэросъёмка",
            Segment = CustomerSegmentEnum.MidMarket,
        });

        _context.SaveChanges();
    }

    public Product Product { get; }

    public IRepository<T> Repository<T>()
        where T : Entity
        => new Repository<T>(_context);

    public void AddSale(
        int id,
        DateOnly date,
        SaleStatusEnum status,
        decimal unitPrice,
        decimal unitCost,
        int quantity = 1,
        int managerId = 1)
    {
        _context.Sales.Add(new Sale
        {
            Id = id,
            Number = $"SO-{id:0000}",
            ManagerId = managerId,
            CustomerId = 1,
            SaleDate = date,
            Status = status,
            Items =
            [
                new SaleItem
                {
                    Id = id * 100,
                    ProductId = Product.Id,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    UnitCost = unitCost,
                },
            ],
        });

        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    private static Manager NewManager(int id, string firstName, string lastName) => new()
    {
        Id = id,
        FirstName = firstName,
        LastName = lastName,
        Team = "Корпоративные продажи",
        Position = "Менеджер по продажам",
        AvatarColor = "#2563eb",
        IsActive = true,
        HiredOn = new DateOnly(2024, 1, 1),
    };
}
