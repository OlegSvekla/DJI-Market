namespace DJI.Core.Analytics;

public readonly record struct Period(DateOnly From, DateOnly To)
{
    public int LengthInDays => To.DayNumber - From.DayNumber + 1;

    public Period Previous() => new(From.AddDays(-LengthInDays), From.AddDays(-1));

    public bool Contains(DateOnly date) => date >= From && date <= To;

    public IEnumerable<DateOnly> Days()
    {
        for (var date = From; date <= To; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    public override string ToString() => $"{From:yyyy-MM-dd}..{To:yyyy-MM-dd}";
}
