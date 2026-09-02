using DJI.Contracts.Enums;
using DJI.Contracts.Rss;

namespace DJI.Bl.Services;

public static class ManagerRanking
{
    public static List<ManagerRatingItemRs> Order(
        IEnumerable<ManagerRatingItemRs> rows,
        ManagerSortByEnum sortBy)
        => [.. rows
            .OrderByDescending(row => row.SalesCount > 0)
            .ThenByDescending(row => SortKey(row, sortBy))
            .ThenBy(row => row.Name, StringComparer.OrdinalIgnoreCase)];

    public static List<ManagerRatingItemRs> AssignPositions(
        List<ManagerRatingItemRs> ordered,
        ManagerSortByEnum sortBy)
    {
        var ranked = new List<ManagerRatingItemRs>(ordered.Count);
        var position = 0;
        decimal? previousKey = null;
        var previousHadSales = false;

        for (var index = 0; index < ordered.Count; index++)
        {
            var row = ordered[index];
            var key = SortKey(row, sortBy);
            var hasSales = row.SalesCount > 0;

            if (index == 0 || key != previousKey || hasSales != previousHadSales)
            {
                position = index + 1;
                previousKey = key;
                previousHadSales = hasSales;
            }

            ranked.Add(row with { Position = position });
        }

        return ranked;
    }

    private static decimal SortKey(ManagerRatingItemRs row, ManagerSortByEnum sortBy)
        => sortBy switch
        {
            ManagerSortByEnum.AverageCheck => row.AverageCheck ?? 0m,
            ManagerSortByEnum.Revenue => row.Revenue,
            _ => row.GrossProfit,
        };
}
