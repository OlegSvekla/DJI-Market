using DJI.Bl.Periods;
using DJI.Contracts.Enums;
using DJI.Contracts.Rqs;
using DJI.Core.Exceptions;

namespace DJI.Tests.Periods;

public class PeriodResolverTests
{
    private static readonly DateOnly Today = new(2026, 3, 18);

    private readonly PeriodResolver _resolver = new(new FixedTimeProvider(Today));

    [Fact]
    public void Today_IsSingleDay()
    {
        var period = _resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.Today });

        Assert.Equal(Today, period.From);
        Assert.Equal(Today, period.To);
        Assert.Equal(1, period.LengthInDays);
    }

    [Fact]
    public void Last7Days_IncludesTodayAndCoversSevenDays()
    {
        var period = _resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.Last7Days });

        Assert.Equal(new DateOnly(2026, 3, 12), period.From);
        Assert.Equal(Today, period.To);
        Assert.Equal(7, period.LengthInDays);
    }

    [Fact]
    public void Last30Days_CoversThirtyDays()
    {
        var period = _resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.Last30Days });

        Assert.Equal(30, period.LengthInDays);
        Assert.Equal(Today, period.To);
    }

    [Fact]
    public void ThisMonth_StartsOnFirstDayAndEndsToday()
    {
        var period = _resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.ThisMonth });

        Assert.Equal(new DateOnly(2026, 3, 1), period.From);
        Assert.Equal(Today, period.To);
    }

    [Fact]
    public void LastMonth_CoversWholePreviousMonth()
    {
        var period = _resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.LastMonth });

        Assert.Equal(new DateOnly(2026, 2, 1), period.From);
        Assert.Equal(new DateOnly(2026, 2, 28), period.To);
    }

    [Fact]
    public void LastMonth_HandlesJanuary()
    {
        var resolver = new PeriodResolver(new FixedTimeProvider(new DateOnly(2026, 1, 10)));

        var period = resolver.Resolve(new PeriodRq { Preset = PeriodPresetEnum.LastMonth });

        Assert.Equal(new DateOnly(2025, 12, 1), period.From);
        Assert.Equal(new DateOnly(2025, 12, 31), period.To);
    }

    [Fact]
    public void Custom_UsesBothBoundaries()
    {
        var period = _resolver.Resolve(new PeriodRq
        {
            Preset = PeriodPresetEnum.Custom,
            From = new DateOnly(2026, 1, 1),
            To = new DateOnly(2026, 1, 31),
        });

        Assert.Equal(new DateOnly(2026, 1, 1), period.From);
        Assert.Equal(new DateOnly(2026, 1, 31), period.To);
    }

    [Fact]
    public void Custom_WithoutBoundaries_IsRejected()
    {
        var request = new PeriodRq { Preset = PeriodPresetEnum.Custom };

        Assert.Throws<DomainValidationException>(() => _resolver.Resolve(request));
    }

    [Fact]
    public void Custom_WithReversedBoundaries_IsRejected()
    {
        var request = new PeriodRq
        {
            Preset = PeriodPresetEnum.Custom,
            From = new DateOnly(2026, 3, 10),
            To = new DateOnly(2026, 3, 1),
        };

        Assert.Throws<DomainValidationException>(() => _resolver.Resolve(request));
    }

    [Fact]
    public void Custom_WithAbsurdlyLongRange_IsRejected()
    {
        var request = new PeriodRq
        {
            Preset = PeriodPresetEnum.Custom,
            From = new DateOnly(2000, 1, 1),
            To = Today,
        };

        Assert.Throws<DomainValidationException>(() => _resolver.Resolve(request));
    }

    private sealed class FixedTimeProvider(DateOnly today) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
            => new(today.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);
    }
}
