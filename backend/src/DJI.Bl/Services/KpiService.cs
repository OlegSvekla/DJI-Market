using DJI.Bl.Mappers;
using DJI.Bl.Models;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Core.Enums;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface IKpiService
{
    Task<KpiRs> GetAsync(Period period, CancellationToken ct = default);
}

public class KpiService(
    IRepository<SaleItem> saleItems,
    IRepository<Sale> sales) : IKpiService
{
    public async Task<KpiRs> GetAsync(Period period, CancellationToken ct = default)
    {
        var current = await GetTotalsAsync(period, ct);
        var previous = await GetTotalsAsync(period.Previous(), ct);
        var topManager = await GetTopManagerAsync(period, ct);

        return KpiMapper.ToKpi(period, current, previous, topManager);
    }

    private async Task<PeriodTotals> GetTotalsAsync(Period period, CancellationToken ct)
    {
        var money = await saleItems.Query()
            .InPeriod(period)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Revenue = group.Sum(item =>
                    item.Sale.Status == SaleStatusEnum.Paid ? item.Quantity * item.UnitPrice : 0m),
                Cost = group.Sum(item =>
                    item.Sale.Status == SaleStatusEnum.Paid ? item.Quantity * item.UnitCost : 0m),
                Refunded = group.Sum(item =>
                    item.Sale.Status == SaleStatusEnum.Refunded ? item.Quantity * item.UnitPrice : 0m),
            })
            .FirstOrDefaultAsync(ct);

        var counts = await sales.Query()
            .InPeriod(period)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Paid = group.Count(sale => sale.Status == SaleStatusEnum.Paid),
                Cancelled = group.Count(sale => sale.Status == SaleStatusEnum.Cancelled),
                Refunded = group.Count(sale => sale.Status == SaleStatusEnum.Refunded),
            })
            .FirstOrDefaultAsync(ct);

        if (money is null && counts is null)
        {
            return PeriodTotals.Empty;
        }

        return new PeriodTotals(
            money?.Revenue ?? 0m,
            money?.Cost ?? 0m,
            money?.Refunded ?? 0m,
            counts?.Paid ?? 0,
            counts?.Cancelled ?? 0,
            counts?.Refunded ?? 0);
    }

    private async Task<TopManagerRs?> GetTopManagerAsync(Period period, CancellationToken ct)
    {
        var top = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => new
            {
                item.Sale.ManagerId,
                item.Sale.Manager.FirstName,
                item.Sale.Manager.LastName,
                item.Sale.Manager.Team,
                item.Sale.Manager.AvatarColor,
                item.Sale.Manager.IsActive,
            })
            .Select(group => new
            {
                group.Key,
                GrossProfit = group.Sum(item => item.Quantity * (item.UnitPrice - item.UnitCost)),
            })
            .OrderByDescending(manager => manager.GrossProfit)
            .FirstOrDefaultAsync(ct);

        if (top is null)
        {
            return null;
        }

        var profile = new ManagerProfile(
            top.Key.ManagerId,
            top.Key.FirstName,
            top.Key.LastName,
            top.Key.Team,
            top.Key.AvatarColor,
            top.Key.IsActive);

        return ManagerMapper.ToTopManager(profile, top.GrossProfit);
    }
}
