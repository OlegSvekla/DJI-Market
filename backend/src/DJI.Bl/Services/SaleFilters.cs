using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Core.Enums;

namespace DJI.Bl.Services;

internal static class SaleFilters
{
    public static IQueryable<SaleItem> InPeriod(this IQueryable<SaleItem> items, Period period)
        => items.Where(item => item.Sale.SaleDate >= period.From && item.Sale.SaleDate <= period.To);

    public static IQueryable<Sale> InPeriod(this IQueryable<Sale> sales, Period period)
        => sales.Where(sale => sale.SaleDate >= period.From && sale.SaleDate <= period.To);

    public static IQueryable<SaleItem> Paid(this IQueryable<SaleItem> items)
        => items.Where(item => item.Sale.Status == SaleStatusEnum.Paid);

    public static IQueryable<Sale> Paid(this IQueryable<Sale> sales)
        => sales.Where(sale => sale.Status == SaleStatusEnum.Paid);
}
