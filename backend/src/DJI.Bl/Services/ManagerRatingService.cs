using DJI.Bl.Mappers;
using DJI.Bl.Models;
using DJI.Contracts.Enums;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface IManagerRatingService
{
    Task<ManagerRatingRs> GetAsync(
        Period period,
        ManagerSortByEnum sortBy,
        int? limit,
        CancellationToken ct = default);
}

public class ManagerRatingService(
    IRepository<SaleItem> saleItems,
    IRepository<Sale> sales,
    IRepository<Manager> managers) : IManagerRatingService
{
    private const int SparkBuckets = 12;

    public async Task<ManagerRatingRs> GetAsync(
        Period period,
        ManagerSortByEnum sortBy,
        int? limit,
        CancellationToken ct = default)
    {
        var current = await AggregateAsync(period, ct);
        var previous = await AggregateAsync(period.Previous(), ct);
        var sparks = await BuildSparksAsync(period, ct);

        var profiles = await managers.GetAllAsync(
            manager => new ManagerProfile(
                manager.Id,
                manager.FirstName,
                manager.LastName,
                manager.Team,
                manager.AvatarColor,
                manager.IsActive),
            ct: ct);

        var rows = profiles
            .Select(profile => ManagerMapper.ToRatingItem(
                profile,
                current.GetValueOrDefault(profile.Id, ManagerTotals.Empty),
                previous.GetValueOrDefault(profile.Id, ManagerTotals.Empty),
                sparks.GetValueOrDefault(profile.Id) ?? []))
            .ToList();

        var ordered = ManagerRanking.Order(rows, sortBy);

        if (limit is > 0)
        {
            ordered = [.. ordered.Take(limit.Value)];
        }

        return new ManagerRatingRs(
            KpiMapper.ToPeriod(period),
            KpiMapper.ToPeriod(period.Previous()),
            ManagerRanking.AssignPositions(ordered, sortBy));
    }

    private async Task<Dictionary<int, ManagerTotals>> AggregateAsync(Period period, CancellationToken ct)
    {
        var money = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => item.Sale.ManagerId)
            .Select(group => new
            {
                ManagerId = group.Key,
                Revenue = group.Sum(item => item.Quantity * item.UnitPrice),
                Cost = group.Sum(item => item.Quantity * item.UnitCost),
            })
            .ToListAsync(ct);

        var counts = await sales.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(sale => sale.ManagerId)
            .Select(group => new { ManagerId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(row => row.ManagerId, row => row.Count, ct);

        return money.ToDictionary(
            row => row.ManagerId,
            row => new ManagerTotals(row.Revenue, row.Cost, counts.GetValueOrDefault(row.ManagerId)));
    }

    private async Task<Dictionary<int, decimal[]>> BuildSparksAsync(Period period, CancellationToken ct)
    {
        var daily = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => new { item.Sale.ManagerId, item.Sale.SaleDate })
            .Select(group => new
            {
                group.Key.ManagerId,
                group.Key.SaleDate,
                Profit = group.Sum(item => item.Quantity * (item.UnitPrice - item.UnitCost)),
            })
            .ToListAsync(ct);

        return daily
            .GroupBy(row => row.ManagerId)
            .ToDictionary(
                group => group.Key,
                group => group.Aggregate(new decimal[SparkBuckets], (buckets, row) =>
                {
                    buckets[BucketOf(row.SaleDate, period)] += row.Profit;

                    return buckets;
                }));
    }

    private static int BucketOf(DateOnly date, Period period)
    {
        var offset = date.DayNumber - period.From.DayNumber;

        return Math.Clamp(offset * SparkBuckets / period.LengthInDays, 0, SparkBuckets - 1);
    }
}
