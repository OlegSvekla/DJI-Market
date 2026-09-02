using DJI.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DJI.Infrastructure.Persistence;

public class DatabaseInitializer(
    DjiDbContext context,
    DataSeeder seeder,
    IOptions<StartupOptions> options)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (options.Value.ApplyMigrations)
        {
            await context.Database.MigrateAsync(ct);
        }

        if (options.Value.RunSeed)
        {
            await seeder.SeedAsync(ct);
        }
    }
}
