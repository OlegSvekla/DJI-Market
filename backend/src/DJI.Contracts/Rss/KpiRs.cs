namespace DJI.Contracts.Rss;

public record PeriodRs(DateOnly From, DateOnly To);

public record MetricRs(decimal? Current, decimal? Previous, decimal? ChangeRate);

public record TopManagerRs(
    int Id,
    string Name,
    string Initials,
    string AvatarColor,
    decimal GrossProfit);

public record KpiRs(
    PeriodRs Period,
    PeriodRs PreviousPeriod,
    MetricRs Revenue,
    MetricRs GrossProfit,
    MetricRs Margin,
    MetricRs SalesCount,
    MetricRs AverageCheck,
    MetricRs RefundedAmount,
    decimal? RefundRate,
    int CancelledCount,
    int RefundedCount,
    TopManagerRs? TopManager);
