using DJI.Bl.Services;
using DJI.Contracts.Enums;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Core.Enums;

namespace DJI.Tests.Analytics;

public class ManagerRatingServiceTests
{
    private static readonly Period March = new(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31));

    [Fact]
    public async Task Rating_RanksByGrossProfit()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Paid, unitPrice: 10_000m, unitCost: 9_000m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 3, 6), SaleStatusEnum.Paid, unitPrice: 5_000m, unitCost: 1_000m, managerId: 2);

        var rating = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);

        Assert.Equal(2, rating.Items[0].ManagerId);
        Assert.Equal(1, rating.Items[0].Position);
        Assert.Equal(4_000m, rating.Items[0].GrossProfit);
    }

    [Fact]
    public async Task Rating_ByAverageCheck_PutsBigDealsFirst()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 3, 6), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m, managerId: 1);
        db.AddSale(3, new DateOnly(2026, 3, 7), SaleStatusEnum.Paid, unitPrice: 4_000m, unitCost: 3_000m, managerId: 2);

        var byProfit = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);
        var byCheck = await Service(db).GetAsync(March, ManagerSortByEnum.AverageCheck, limit: null);

        Assert.Equal(1, byProfit.Items[0].ManagerId);
        Assert.Equal(2, byCheck.Items[0].ManagerId);
        Assert.Equal(4_000m, byCheck.Items[0].AverageCheck);
    }

    [Fact]
    public async Task Rating_KeepsManagersWithoutSales()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m, managerId: 1);

        var rating = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);

        var idle = rating.Items.Single(item => item.ManagerId == 2);

        Assert.Equal(0, idle.SalesCount);
        Assert.Null(idle.AverageCheck);
        Assert.Null(idle.Margin);
        Assert.Equal(2, rating.Items.Count);
    }

    [Fact]
    public async Task Rating_IgnoresCancelledAndRefundedSales()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Cancelled, unitPrice: 9_000m, unitCost: 1_000m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 3, 6), SaleStatusEnum.Refunded, unitPrice: 8_000m, unitCost: 1_000m, managerId: 1);

        var rating = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);
        var manager = rating.Items.Single(item => item.ManagerId == 1);

        Assert.Equal(0, manager.SalesCount);
        Assert.Equal(0m, manager.Revenue);
        Assert.Equal(0m, manager.GrossProfit);
    }

    [Fact]
    public async Task Rating_ComparesWithPreviousPeriod()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Paid, unitPrice: 2_000m, unitCost: 1_000m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 2, 5), SaleStatusEnum.Paid, unitPrice: 1_500m, unitCost: 1_000m, managerId: 1);

        var rating = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);
        var manager = rating.Items.Single(item => item.ManagerId == 1);

        Assert.Equal(1_000m, manager.GrossProfit);
        Assert.Equal(1m, manager.GrossProfitChange);
    }

    [Fact]
    public async Task Rating_BuildsSparklineOverThePeriod()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 2), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 3, 30), SaleStatusEnum.Paid, unitPrice: 3_000m, unitCost: 1_000m, managerId: 1);

        var rating = await Service(db).GetAsync(March, ManagerSortByEnum.GrossProfit, limit: null);
        var manager = rating.Items.Single(item => item.ManagerId == 1);

        Assert.Equal(12, manager.Spark.Count);
        Assert.Equal(2_500m, manager.Spark.Sum());

        Assert.True(manager.Spark[0] > 0);
        Assert.True(manager.Spark[^1] > 0);
    }

    private static ManagerRatingService Service(AnalyticsTestContext db)
        => new(db.Repository<SaleItem>(), db.Repository<Sale>(), db.Repository<Manager>());
}
