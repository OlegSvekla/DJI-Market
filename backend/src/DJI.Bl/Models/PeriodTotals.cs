using DJI.Core.Analytics;

namespace DJI.Bl.Models;

public sealed record PeriodTotals(
    decimal Revenue,
    decimal Cost,
    decimal RefundedAmount,
    int PaidCount,
    int CancelledCount,
    int RefundedCount)
{
    public static readonly PeriodTotals Empty = new(0m, 0m, 0m, 0, 0, 0);

    public decimal GrossProfit => KpiMath.GrossProfit(Revenue, Cost);

    public decimal? Margin => KpiMath.Margin(Revenue, GrossProfit);

    public decimal? AverageCheck => KpiMath.AverageCheck(Revenue, PaidCount);
}
