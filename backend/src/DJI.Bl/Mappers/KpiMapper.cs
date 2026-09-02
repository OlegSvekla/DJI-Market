using DJI.Bl.Models;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;

namespace DJI.Bl.Mappers;

public static class KpiMapper
{
    public static KpiRs ToKpi(
        Period period,
        PeriodTotals current,
        PeriodTotals previous,
        TopManagerRs? topManager) => new(
        ToPeriod(period),
        ToPeriod(period.Previous()),
        Metric(current.Revenue, previous.Revenue),
        Metric(current.GrossProfit, previous.GrossProfit),
        Metric(current.Margin, previous.Margin),
        Metric(current.PaidCount, previous.PaidCount),
        Metric(current.AverageCheck, previous.AverageCheck),
        Metric(current.RefundedAmount, previous.RefundedAmount),
        KpiMath.RefundRate(current.RefundedAmount, current.Revenue),
        current.CancelledCount,
        current.RefundedCount,
        topManager);

    public static PeriodRs ToPeriod(Period period) => new(period.From, period.To);

    private static MetricRs Metric(decimal? current, decimal? previous) => new(
        current,
        previous,
        current is null || previous is null ? null : KpiMath.ChangeRate(current.Value, previous.Value));
}
