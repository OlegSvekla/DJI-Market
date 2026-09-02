using DJI.Core.Analytics;

namespace DJI.Tests.Analytics;

public class KpiMathTests
{
    [Fact]
    public void GrossProfit_IsRevenueMinusCost()
    {
        Assert.Equal(400m, KpiMath.GrossProfit(1_000m, 600m));
    }

    [Fact]
    public void GrossProfit_CanBeNegative_WhenDealSoldBelowCost()
    {
        Assert.Equal(-150m, KpiMath.GrossProfit(1_000m, 1_150m));
    }

    [Fact]
    public void Margin_IsShareOfRevenue()
    {
        Assert.Equal(0.4m, KpiMath.Margin(1_000m, 400m));
    }

    [Fact]
    public void Margin_IsNull_WhenNoRevenue()
    {
        Assert.Null(KpiMath.Margin(0m, 0m));
    }

    [Fact]
    public void AverageCheck_DividesRevenueBySalesCount()
    {
        Assert.Equal(250m, KpiMath.AverageCheck(1_000m, 4));
    }

    [Fact]
    public void AverageCheck_IsNull_WhenNoSales()
    {
        Assert.Null(KpiMath.AverageCheck(0m, 0));
    }

    [Fact]
    public void ChangeRate_ReturnsGrowthShare()
    {
        Assert.Equal(0.25m, KpiMath.ChangeRate(1_250m, 1_000m));
    }

    [Fact]
    public void ChangeRate_ReturnsNegative_OnDecline()
    {
        Assert.Equal(-0.2m, KpiMath.ChangeRate(800m, 1_000m));
    }

    [Fact]
    public void ChangeRate_IsNull_WhenPreviousPeriodWasEmpty()
    {
        Assert.Null(KpiMath.ChangeRate(500m, 0m));
    }

    [Fact]
    public void RefundRate_IsShareOfReturnedMoney()
    {
        Assert.Equal(0.1m, KpiMath.RefundRate(100m, 900m));
    }

    [Fact]
    public void RefundRate_IsNull_WhenNothingHappened()
    {
        Assert.Null(KpiMath.RefundRate(0m, 0m));
    }
}
