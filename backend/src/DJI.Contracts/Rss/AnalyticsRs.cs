using DJI.Contracts.Enums;
using DJI.Core.Enums;

namespace DJI.Contracts.Rss;

public record TimeSeriesPointRs(
    DateOnly Date,
    decimal Revenue,
    decimal GrossProfit,
    int SalesCount);

public record TimeSeriesRs(
    PeriodRs Period,
    TimeGranularityEnum Granularity,
    IReadOnlyList<TimeSeriesPointRs> Points);

public record CategorySliceRs(
    int CategoryId,
    string Name,
    decimal Revenue,
    decimal GrossProfit,
    decimal? Margin,
    decimal Share);

public record TopProductRs(
    int ProductId,
    string Name,
    string Category,
    int Quantity,
    decimal Revenue,
    decimal GrossProfit,
    decimal? Margin);

public record ManagerBriefRs(
    int Id,
    string Name,
    string Initials,
    string AvatarColor);

public record RecentSaleRs(
    int Id,
    string Number,
    DateOnly Date,
    ManagerBriefRs Manager,
    string CustomerCompany,
    string CustomerName,
    SaleStatusEnum Status,
    int ItemsCount,
    string ItemsPreview,
    decimal Amount,
    decimal GrossProfit);

public record PagedRs<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total);

public record FilterOptionRs(int Id, string Name);

public record FiltersRs(
    IReadOnlyList<FilterOptionRs> Managers,
    IReadOnlyList<FilterOptionRs> Categories,
    IReadOnlyList<string> Statuses,
    DateOnly? FirstSaleDate,
    DateOnly? LastSaleDate);
