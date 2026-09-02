using DJI.Bl.Periods;
using DJI.Bl.Services;
using DJI.Contracts.Enums;
using DJI.Contracts.Rqs;
using DJI.Contracts.Rss;
using Microsoft.AspNetCore.Mvc;

namespace DJI.Web.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(
    IPeriodResolver periodResolver,
    IKpiService kpiService,
    IManagerRatingService managerRatingService,
    ITimeSeriesService timeSeriesService,
    ICatalogAnalyticsService catalogAnalyticsService) : ControllerBase
{
    [HttpGet("kpi")]
    [ProducesResponseType(typeof(KpiRs), StatusCodes.Status200OK)]
    public async Task<ActionResult<KpiRs>> GetKpi([FromQuery] PeriodRq request, CancellationToken ct)
        => Ok(await kpiService.GetAsync(periodResolver.Resolve(request), ct));

    [HttpGet("managers")]
    [ProducesResponseType(typeof(ManagerRatingRs), StatusCodes.Status200OK)]
    public async Task<ActionResult<ManagerRatingRs>> GetManagers(
        [FromQuery] PeriodRq request,
        [FromQuery] ManagerSortByEnum sortBy,
        [FromQuery] int? limit,
        CancellationToken ct)
        => Ok(await managerRatingService.GetAsync(periodResolver.Resolve(request), sortBy, limit, ct));

    [HttpGet("timeseries")]
    [ProducesResponseType(typeof(TimeSeriesRs), StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeSeriesRs>> GetTimeSeries(
        [FromQuery] PeriodRq request,
        [FromQuery] TimeGranularityEnum granularity,
        CancellationToken ct)
        => Ok(await timeSeriesService.GetAsync(periodResolver.Resolve(request), granularity, ct));

    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<CategorySliceRs>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategorySliceRs>>> GetCategories(
        [FromQuery] PeriodRq request,
        CancellationToken ct)
        => Ok(await catalogAnalyticsService.GetCategoriesAsync(periodResolver.Resolve(request), ct));

    [HttpGet("top-products")]
    [ProducesResponseType(typeof(IReadOnlyList<TopProductRs>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TopProductRs>>> GetTopProducts(
        [FromQuery] PeriodRq request,
        [FromQuery] int limit,
        CancellationToken ct)
        => Ok(await catalogAnalyticsService.GetTopProductsAsync(
            periodResolver.Resolve(request),
            limit <= 0 ? 10 : Math.Min(limit, 50),
            ct));
}
