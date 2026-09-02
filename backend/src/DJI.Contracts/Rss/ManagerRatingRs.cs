namespace DJI.Contracts.Rss;

public record ManagerRatingItemRs(
    int Position,
    int ManagerId,
    string Name,
    string Initials,
    string AvatarColor,
    string Team,
    bool IsActive,
    int SalesCount,
    decimal Revenue,
    decimal GrossProfit,
    decimal? AverageCheck,
    decimal? Margin,
    decimal? GrossProfitChange,
    decimal? AverageCheckChange,
    IReadOnlyList<decimal> Spark);

public record ManagerRatingRs(
    PeriodRs Period,
    PeriodRs PreviousPeriod,
    IReadOnlyList<ManagerRatingItemRs> Items);
