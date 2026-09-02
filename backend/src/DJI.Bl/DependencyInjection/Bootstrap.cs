using DJI.Bl.Periods;
using DJI.Bl.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DJI.Bl.DependencyInjection;

public static class Bootstrap
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IPeriodResolver, PeriodResolver>();
        services.AddScoped<IKpiService, KpiService>();
        services.AddScoped<IManagerRatingService, ManagerRatingService>();
        services.AddScoped<ITimeSeriesService, TimeSeriesService>();
        services.AddScoped<ICatalogAnalyticsService, CatalogAnalyticsService>();
        services.AddScoped<ISalesFeedService, SalesFeedService>();
        services.AddScoped<IFiltersService, FiltersService>();

        return services;
    }
}
