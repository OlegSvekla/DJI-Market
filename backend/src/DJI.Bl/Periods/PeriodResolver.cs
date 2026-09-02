using DJI.Contracts.Enums;
using DJI.Contracts.Rqs;
using DJI.Core.Analytics;
using DJI.Core.Constants;
using DJI.Core.Exceptions;

namespace DJI.Bl.Periods;

public interface IPeriodResolver
{
    Period Resolve(PeriodRq request);
}

public class PeriodResolver(TimeProvider timeProvider) : IPeriodResolver
{
    private const int MaxRangeInDays = 366 * 3;

    private const int WeekLengthInDays = 7;

    private const int MonthLengthInDays = 30;

    public Period Resolve(PeriodRq request)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var period = request.Preset switch
        {
            PeriodPresetEnum.Today => new Period(today, today),
            PeriodPresetEnum.Last7Days => new Period(today.AddDays(-(WeekLengthInDays - 1)), today),
            PeriodPresetEnum.Last30Days => new Period(today.AddDays(-(MonthLengthInDays - 1)), today),
            PeriodPresetEnum.ThisMonth => new Period(FirstDayOf(today), today),
            PeriodPresetEnum.LastMonth => ResolveLastMonth(today),
            PeriodPresetEnum.Custom => ResolveCustom(request),
            _ => throw new DomainValidationException(
                string.Format(ErrorMessages.UnknownPeriodPresetFormat, request.Preset)),
        };

        return period.LengthInDays > MaxRangeInDays
            ? throw new DomainValidationException(
                string.Format(ErrorMessages.PeriodTooLongFormat, MaxRangeInDays, period.LengthInDays))
            : period;
    }

    private static Period ResolveLastMonth(DateOnly today)
    {
        var firstDayOfThisMonth = FirstDayOf(today);

        return new Period(firstDayOfThisMonth.AddMonths(-1), firstDayOfThisMonth.AddDays(-1));
    }

    private static Period ResolveCustom(PeriodRq request)
    {
        if (request.From is null || request.To is null)
        {
            throw new DomainValidationException(ErrorMessages.CustomPeriodRequiresBothBounds);
        }

        if (request.From > request.To)
        {
            throw new DomainValidationException(ErrorMessages.PeriodStartAfterEnd);
        }

        return new Period(request.From.Value, request.To.Value);
    }

    private static DateOnly FirstDayOf(DateOnly date) => new(date.Year, date.Month, 1);
}
