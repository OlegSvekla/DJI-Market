using DJI.Core.Constants;
using DJI.Infrastructure.Persistence;
using DJI.Infrastructure.Repositories;
using DJI.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DJI.Infrastructure.DependencyInjection;

public static class Bootstrap
{
    private const string ConnectionStringName = "Postgres";

    private const int ConnectionRetryCount = 6;

    private static readonly TimeSpan ConnectionRetryDelay = TimeSpan.FromSeconds(5);

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));
        services.Configure<StartupOptions>(configuration.GetSection(StartupOptions.SectionName));

        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                string.Format(ErrorMessages.ConnectionStringMissingFormat, ConnectionStringName));

        services.AddDbContext<DjiDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(
                maxRetryCount: ConnectionRetryCount,
                maxRetryDelay: ConnectionRetryDelay,
                errorCodesToAdd: null))
            .UseSnakeCaseNamingConvention());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<DataSeeder>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
