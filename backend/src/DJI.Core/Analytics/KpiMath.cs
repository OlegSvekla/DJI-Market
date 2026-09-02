namespace DJI.Core.Analytics;

public static class KpiMath
{
    public static decimal GrossProfit(decimal revenue, decimal cost) => revenue - cost;

    public static decimal? Margin(decimal revenue, decimal grossProfit)
        => revenue == 0m ? null : grossProfit / revenue;

    public static decimal? AverageCheck(decimal revenue, int salesCount)
        => salesCount == 0 ? null : revenue / salesCount;

    public static decimal? ChangeRate(decimal current, decimal previous)
        => previous == 0m ? null : (current - previous) / Math.Abs(previous);

    public static decimal? RefundRate(decimal refundedAmount, decimal paidRevenue)
    {
        var total = refundedAmount + paidRevenue;

        return total == 0m ? null : refundedAmount / total;
    }
}
