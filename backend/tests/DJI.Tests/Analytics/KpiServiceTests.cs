using DJI.Bl.Services;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Core.Enums;

namespace DJI.Tests.Analytics;

public class KpiServiceTests
{
    private static readonly Period March = new(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31));

    [Fact]
    public async Task Revenue_CountsOnlyPaidSales()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 10), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 600m);
        db.AddSale(2, new DateOnly(2026, 3, 11), SaleStatusEnum.Cancelled, unitPrice: 5_000m, unitCost: 3_000m);
        db.AddSale(3, new DateOnly(2026, 3, 12), SaleStatusEnum.Refunded, unitPrice: 7_000m, unitCost: 4_000m);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(1_000m, kpi.Revenue.Current);
        Assert.Equal(400m, kpi.GrossProfit.Current);
        Assert.Equal(1, kpi.SalesCount.Current);
    }

    [Fact]
    public async Task Refunded_IsReportedSeparatelyFromRevenue()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 10), SaleStatusEnum.Paid, unitPrice: 9_000m, unitCost: 5_000m);
        db.AddSale(2, new DateOnly(2026, 3, 12), SaleStatusEnum.Refunded, unitPrice: 1_000m, unitCost: 600m);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(9_000m, kpi.Revenue.Current);
        Assert.Equal(1_000m, kpi.RefundedAmount.Current);
        Assert.Equal(1, kpi.RefundedCount);

        Assert.Equal(0.1m, kpi.RefundRate);
    }

    [Fact]
    public async Task CancelledSales_AreCountedButNotMonetised()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 5), SaleStatusEnum.Cancelled, unitPrice: 4_000m, unitCost: 2_000m);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(0m, kpi.Revenue.Current);
        Assert.Equal(1, kpi.CancelledCount);
        Assert.Equal(0m, kpi.RefundedAmount.Current);
    }

    [Fact]
    public async Task SalesCount_CountsSalesNotItems()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 9), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 600m, quantity: 3);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(1, kpi.SalesCount.Current);
        Assert.Equal(3_000m, kpi.Revenue.Current);
        Assert.Equal(3_000m, kpi.AverageCheck.Current);
    }

    [Fact]
    public async Task SalesOnPeriodBoundaries_AreIncludedExactlyOnce()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 2, 28), SaleStatusEnum.Paid, unitPrice: 100m, unitCost: 50m);
        db.AddSale(2, new DateOnly(2026, 3, 1), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m);
        db.AddSale(3, new DateOnly(2026, 3, 31), SaleStatusEnum.Paid, unitPrice: 2_000m, unitCost: 1_000m);
        db.AddSale(4, new DateOnly(2026, 4, 1), SaleStatusEnum.Paid, unitPrice: 400m, unitCost: 200m);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(3_000m, kpi.Revenue.Current);
        Assert.Equal(2, kpi.SalesCount.Current);
    }

    [Fact]
    public async Task PreviousPeriod_IsComparableAndDoesNotOverlap()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 10), SaleStatusEnum.Paid, unitPrice: 1_200m, unitCost: 600m);
        db.AddSale(2, new DateOnly(2026, 2, 10), SaleStatusEnum.Paid, unitPrice: 1_000m, unitCost: 500m);

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(1_200m, kpi.Revenue.Current);
        Assert.Equal(1_000m, kpi.Revenue.Previous);
        Assert.Equal(0.2m, kpi.Revenue.ChangeRate);
    }

    [Fact]
    public async Task EmptyPeriod_ReturnsZeroMoneyAndNullDerivedMetrics()
    {
        using var db = new AnalyticsTestContext();

        var kpi = await Service(db).GetAsync(March);

        Assert.Equal(0m, kpi.Revenue.Current);
        Assert.Equal(0, kpi.SalesCount.Current);

        Assert.Null(kpi.Margin.Current);
        Assert.Null(kpi.AverageCheck.Current);
        Assert.Null(kpi.TopManager);
    }

    [Fact]
    public async Task TopManager_IsChosenByGrossProfitNotRevenue()
    {
        using var db = new AnalyticsTestContext();

        db.AddSale(1, new DateOnly(2026, 3, 4), SaleStatusEnum.Paid, unitPrice: 10_000m, unitCost: 9_500m, managerId: 1);
        db.AddSale(2, new DateOnly(2026, 3, 5), SaleStatusEnum.Paid, unitPrice: 4_000m, unitCost: 1_000m, managerId: 2);

        var kpi = await Service(db).GetAsync(March);

        Assert.NotNull(kpi.TopManager);
        Assert.Equal(2, kpi.TopManager!.Id);
        Assert.Equal(3_000m, kpi.TopManager.GrossProfit);
    }

    private static KpiService Service(AnalyticsTestContext db)
        => new(db.Repository<SaleItem>(), db.Repository<Sale>());
}
