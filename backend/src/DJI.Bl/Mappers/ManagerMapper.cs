using DJI.Bl.Models;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;

namespace DJI.Bl.Mappers;

public static class ManagerMapper
{
    public static string FullName(string firstName, string lastName) => $"{firstName} {lastName}".Trim();

    public static string Initials(string firstName, string lastName)
    {
        var first = firstName.Length > 0 ? firstName[0] : '?';
        var last = lastName.Length > 0 ? lastName[0] : '?';

        return string.Concat(first, last).ToUpperInvariant();
    }

    public static ManagerBriefRs ToBrief(ManagerProfile manager) => new(
        manager.Id,
        FullName(manager.FirstName, manager.LastName),
        Initials(manager.FirstName, manager.LastName),
        manager.AvatarColor);

    public static TopManagerRs ToTopManager(ManagerProfile manager, decimal grossProfit) => new(
        manager.Id,
        FullName(manager.FirstName, manager.LastName),
        Initials(manager.FirstName, manager.LastName),
        manager.AvatarColor,
        grossProfit);

    public static ManagerRatingItemRs ToRatingItem(
        ManagerProfile manager,
        ManagerTotals current,
        ManagerTotals previous,
        IReadOnlyList<decimal> spark) => new(
        Position: 0,
        ManagerId: manager.Id,
        Name: FullName(manager.FirstName, manager.LastName),
        Initials: Initials(manager.FirstName, manager.LastName),
        AvatarColor: manager.AvatarColor,
        Team: manager.Team,
        IsActive: manager.IsActive,
        SalesCount: current.SalesCount,
        Revenue: current.Revenue,
        GrossProfit: current.GrossProfit,
        AverageCheck: current.AverageCheck,
        Margin: current.Margin,
        GrossProfitChange: KpiMath.ChangeRate(current.GrossProfit, previous.GrossProfit),
        AverageCheckChange: current.AverageCheck is null || previous.AverageCheck is null
            ? null
            : KpiMath.ChangeRate(current.AverageCheck.Value, previous.AverageCheck.Value),
        Spark: spark);
}
