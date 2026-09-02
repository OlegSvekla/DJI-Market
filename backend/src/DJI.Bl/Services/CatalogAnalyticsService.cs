using DJI.Contracts.Rss;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface ICatalogAnalyticsService
{
    Task<IReadOnlyList<CategorySliceRs>> GetCategoriesAsync(Period period, CancellationToken ct = default);

    Task<IReadOnlyList<TopProductRs>> GetTopProductsAsync(Period period, int limit, CancellationToken ct = default);
}

public class CatalogAnalyticsService(IRepository<SaleItem> saleItems) : ICatalogAnalyticsService
{
    public async Task<IReadOnlyList<CategorySliceRs>> GetCategoriesAsync(
        Period period,
        CancellationToken ct = default)
    {
        var rows = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => new { item.Product.CategoryId, item.Product.Category.Name })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Name,
                Revenue = group.Sum(item => item.Quantity * item.UnitPrice),
                Cost = group.Sum(item => item.Quantity * item.UnitCost),
            })
            .ToListAsync(ct);

        var total = rows.Sum(row => row.Revenue);

        return [.. rows
            .Select(row =>
            {
                var grossProfit = KpiMath.GrossProfit(row.Revenue, row.Cost);

                return new CategorySliceRs(
                    row.CategoryId,
                    row.Name,
                    row.Revenue,
                    grossProfit,
                    KpiMath.Margin(row.Revenue, grossProfit),
                    total == 0m ? 0m : row.Revenue / total);
            })
            .OrderByDescending(slice => slice.Revenue)];
    }

    public async Task<IReadOnlyList<TopProductRs>> GetTopProductsAsync(
        Period period,
        int limit,
        CancellationToken ct = default)
    {
        var rows = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => new { item.ProductId, item.Product.Name, CategoryName = item.Product.Category.Name })
            .Select(group => new
            {
                group.Key.ProductId,
                group.Key.Name,
                group.Key.CategoryName,
                Quantity = group.Sum(item => item.Quantity),
                Revenue = group.Sum(item => item.Quantity * item.UnitPrice),
                Cost = group.Sum(item => item.Quantity * item.UnitCost),
            })
            .OrderByDescending(row => row.Revenue)
            .Take(limit)
            .ToListAsync(ct);

        return [.. rows
            .Select(row =>
            {
                var grossProfit = KpiMath.GrossProfit(row.Revenue, row.Cost);

                return new TopProductRs(
                    row.ProductId,
                    row.Name,
                    row.CategoryName,
                    row.Quantity,
                    row.Revenue,
                    grossProfit,
                    KpiMath.Margin(row.Revenue, grossProfit));
            })];
    }
}
