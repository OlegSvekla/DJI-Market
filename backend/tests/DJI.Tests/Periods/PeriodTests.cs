using DJI.Core.Analytics;

namespace DJI.Tests.Periods;

public class PeriodTests
{
    [Fact]
    public void LengthInDays_CountsBothBoundaries()
    {
        var period = new Period(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 7));

        Assert.Equal(7, period.LengthInDays);
    }

    [Fact]
    public void LengthInDays_IsOne_ForSingleDay()
    {
        var today = new DateOnly(2026, 3, 1);

        Assert.Equal(1, new Period(today, today).LengthInDays);
    }

    [Fact]
    public void Previous_IsSameLengthAndEndsDayBeforeStart()
    {
        var period = new Period(new DateOnly(2026, 3, 8), new DateOnly(2026, 3, 14));

        var previous = period.Previous();

        Assert.Equal(new DateOnly(2026, 3, 1), previous.From);
        Assert.Equal(new DateOnly(2026, 3, 7), previous.To);
        Assert.Equal(period.LengthInDays, previous.LengthInDays);
    }

    [Fact]
    public void Previous_DoesNotOverlapCurrentPeriod()
    {
        var period = new Period(new DateOnly(2026, 3, 8), new DateOnly(2026, 3, 14));

        Assert.True(period.Previous().To < period.From);
    }

    [Fact]
    public void Contains_IncludesBothBoundaries()
    {
        var period = new Period(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31));

        Assert.True(period.Contains(new DateOnly(2026, 3, 1)));
        Assert.True(period.Contains(new DateOnly(2026, 3, 31)));
        Assert.False(period.Contains(new DateOnly(2026, 2, 28)));
        Assert.False(period.Contains(new DateOnly(2026, 4, 1)));
    }

    [Fact]
    public void Previous_HandlesMonthAndYearBoundary()
    {
        var january = new Period(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31));

        var previous = january.Previous();

        Assert.Equal(new DateOnly(2025, 12, 1), previous.From);
        Assert.Equal(new DateOnly(2025, 12, 31), previous.To);
    }
}
