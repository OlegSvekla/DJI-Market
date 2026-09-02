using DJI.Bl.Mappers;
using DJI.Bl.Models;
using DJI.Contracts.Enums;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface ITimeSeriesService
{
    Task<TimeSeriesRs> GetAsync(
        Period period,
        TimeGranularityEnum granularity,
        CancellationToken ct = default);
}

public class TimeSeriesService(
    IRepository<SaleItem> saleItems,
    IRepository<Sale> sales) : ITimeSeriesService
{
    private const int MaxDaysForDailySteps = 62;

    private const int MaxDaysForWeeklySteps = 180;

    private const int DaysInWeek = 7;

    public async Task<TimeSeriesRs> GetAsync(
        Period period,
        TimeGranularityEnum granularity,
        CancellationToken ct = default)
    {
        var step = granularity == TimeGranularityEnum.Auto ? PickGranularity(period) : granularity;
        var daily = await GetDailyTotalsAsync(period, ct);

        var points = period.Days()
            .GroupBy(date => StartOfBucket(date, step))
            .OrderBy(bucket => bucket.Key)
            .Select(bucket => ToPoint(
                bucket.Key,
                bucket.Select(date => daily.GetValueOrDefault(date, DailyTotals.Empty))))
            .ToList();

        return new TimeSeriesRs(KpiMapper.ToPeriod(period), step, points);
    }

    private async Task<Dictionary<DateOnly, DailyTotals>> GetDailyTotalsAsync(Period period, CancellationToken ct)
    {
        var money = await saleItems.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(item => item.Sale.SaleDate)
            .Select(group => new
            {
                Date = group.Key,
                Revenue = group.Sum(item => item.Quantity * item.UnitPrice),
                Cost = group.Sum(item => item.Quantity * item.UnitCost),
            })
            .ToListAsync(ct);

        var counts = await sales.Query()
            .InPeriod(period)
            .Paid()
            .GroupBy(sale => sale.SaleDate)
            .Select(group => new { Date = group.Key, Count = group.Count() })
            .ToDictionaryAsync(row => row.Date, row => row.Count, ct);

        return money.ToDictionary(
            row => row.Date,
            row => new DailyTotals(row.Revenue, row.Cost, counts.GetValueOrDefault(row.Date)));
    }

    private static TimeSeriesPointRs ToPoint(DateOnly bucket, IEnumerable<DailyTotals> days)
    {
        var totals = days.ToList();
        var revenue = totals.Sum(day => day.Revenue);
        var cost = totals.Sum(day => day.Cost);

        return new TimeSeriesPointRs(
            bucket,
            revenue,
            KpiMath.GrossProfit(revenue, cost),
            totals.Sum(day => day.SalesCount));
    }

    private static TimeGranularityEnum PickGranularity(Period period)
        => period.LengthInDays switch
        {
            <= MaxDaysForDailySteps => TimeGranularityEnum.Day,
            <= MaxDaysForWeeklySteps => TimeGranularityEnum.Week,
            _ => TimeGranularityEnum.Month,
        };

    private static DateOnly StartOfBucket(DateOnly date, TimeGranularityEnum granularity)
        => granularity switch
        {
            TimeGranularityEnum.Month => new DateOnly(date.Year, date.Month, 1),
            TimeGranularityEnum.Week => date.AddDays(-((int)date.DayOfWeek + DaysInWeek - 1) % DaysInWeek),
            _ => date,
        };
}
