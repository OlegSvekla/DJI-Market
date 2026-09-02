using DJI.Core.Analytics;

namespace DJI.Bl.Models;

public sealed record ManagerTotals(decimal Revenue, decimal Cost, int SalesCount)
{
    public static readonly ManagerTotals Empty = new(0m, 0m, 0);

    public decimal GrossProfit => KpiMath.GrossProfit(Revenue, Cost);

    public decimal? Margin => KpiMath.Margin(Revenue, GrossProfit);

    public decimal? AverageCheck => KpiMath.AverageCheck(Revenue, SalesCount);
}
