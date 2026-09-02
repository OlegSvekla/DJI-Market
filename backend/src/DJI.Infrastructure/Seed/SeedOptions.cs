namespace DJI.Infrastructure.Seed;

public class SeedOptions
{
    public const string SectionName = "Seed";

    public int RandomSeed { get; set; } = 20260901;

    public DateOnly? AnchorDate { get; set; }

    public int MonthsOfHistory { get; set; } = 12;

    public int Managers { get; set; } = 20;

    public int Customers { get; set; } = 80;

    public int Sales { get; set; } = 3500;
}
