namespace DJI.Bl.Models;

public sealed record DailyTotals(decimal Revenue, decimal Cost, int SalesCount)
{
    public static readonly DailyTotals Empty = new(0m, 0m, 0);
}
