namespace DJI.Infrastructure.Persistence;

public class StartupOptions
{
    public const string SectionName = "Startup";

    public bool ApplyMigrations { get; set; } = true;

    public bool RunSeed { get; set; } = true;
}
