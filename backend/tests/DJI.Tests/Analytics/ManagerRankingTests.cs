using DJI.Bl.Services;
using DJI.Contracts.Enums;
using DJI.Contracts.Rss;

namespace DJI.Tests.Analytics;

public class ManagerRankingTests
{
    [Fact]
    public void Order_ByGrossProfit_PutsBiggestProfitFirst()
    {
        var rows = new List<ManagerRatingItemRs>
        {
            Row("Смирнова", salesCount: 10, revenue: 1_000m, grossProfit: 100m),
            Row("Ковалёв", salesCount: 5, revenue: 900m, grossProfit: 300m),
            Row("Орлов", salesCount: 8, revenue: 800m, grossProfit: 200m),
        };

        var ordered = ManagerRanking.Order(rows, ManagerSortByEnum.GrossProfit);

        Assert.Equal(["Ковалёв", "Орлов", "Смирнова"], ordered.Select(row => row.Name));
    }

    [Fact]
    public void Order_ByAverageCheck_UsesDifferentWinnerThanProfit()
    {
        var rows = new List<ManagerRatingItemRs>
        {
            Row("Смирнова", salesCount: 100, revenue: 100_000m, grossProfit: 20_000m),
            Row("Ковалёв", salesCount: 2, revenue: 60_000m, grossProfit: 9_000m),
        };

        var byProfit = ManagerRanking.Order(rows, ManagerSortByEnum.GrossProfit);
        var byCheck = ManagerRanking.Order(rows, ManagerSortByEnum.AverageCheck);

        Assert.Equal("Смирнова", byProfit[0].Name);
        Assert.Equal("Ковалёв", byCheck[0].Name);
    }

    [Fact]
    public void Order_PutsManagersWithoutSalesLast_EvenThoughTheirProfitIsZero()
    {
        var rows = new List<ManagerRatingItemRs>
        {
            Row("Без продаж", salesCount: 0, revenue: 0m, grossProfit: 0m),
            Row("В минусе", salesCount: 3, revenue: 500m, grossProfit: -100m),
        };

        var ordered = ManagerRanking.Order(rows, ManagerSortByEnum.GrossProfit);

        Assert.Equal("В минусе", ordered[0].Name);
        Assert.Equal("Без продаж", ordered[1].Name);
    }

    [Fact]
    public void AssignPositions_GivesEqualResultsTheSamePosition()
    {
        var rows = new List<ManagerRatingItemRs>
        {
            Row("Первый", salesCount: 10, revenue: 1_000m, grossProfit: 500m),
            Row("Второй", salesCount: 8, revenue: 900m, grossProfit: 300m),
            Row("Третий", salesCount: 6, revenue: 700m, grossProfit: 300m),
            Row("Четвёртый", salesCount: 4, revenue: 500m, grossProfit: 100m),
        };

        var ranked = ManagerRanking.AssignPositions(
            ManagerRanking.Order(rows, ManagerSortByEnum.GrossProfit),
            ManagerSortByEnum.GrossProfit);

        Assert.Equal([1, 2, 2, 4], ranked.Select(row => row.Position));
    }

    [Fact]
    public void AssignPositions_DoesNotMergeZeroProfitWithNoSales()
    {
        var rows = new List<ManagerRatingItemRs>
        {
            Row("Отработал в ноль", salesCount: 4, revenue: 1_000m, grossProfit: 0m),
            Row("Не продавал", salesCount: 0, revenue: 0m, grossProfit: 0m),
        };

        var ranked = ManagerRanking.AssignPositions(
            ManagerRanking.Order(rows, ManagerSortByEnum.GrossProfit),
            ManagerSortByEnum.GrossProfit);

        Assert.Equal([1, 2], ranked.Select(row => row.Position));
    }

    private static ManagerRatingItemRs Row(string name, int salesCount, decimal revenue, decimal grossProfit)
        => new(
            Position: 0,
            ManagerId: name.GetHashCode(),
            Name: name,
            Initials: name[..1],
            AvatarColor: "#000000",
            Team: "Тест",
            IsActive: true,
            SalesCount: salesCount,
            Revenue: revenue,
            GrossProfit: grossProfit,
            AverageCheck: salesCount == 0 ? null : revenue / salesCount,
            Margin: revenue == 0m ? null : grossProfit / revenue,
            GrossProfitChange: null,
            AverageCheckChange: null,
            Spark: []);
}
