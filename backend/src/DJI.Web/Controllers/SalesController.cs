using DJI.Bl.Periods;
using DJI.Bl.Services;
using DJI.Contracts.Rqs;
using DJI.Contracts.Rss;
using DJI.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DJI.Web.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController(
    IPeriodResolver periodResolver,
    ISalesFeedService salesFeedService) : ControllerBase
{
    [HttpGet("recent")]
    [ProducesResponseType(typeof(PagedRs<RecentSaleRs>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedRs<RecentSaleRs>>> GetRecent(
        [FromQuery] PeriodRq request,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] int? managerId,
        [FromQuery] SaleStatusEnum? status,
        CancellationToken ct)
        => Ok(await salesFeedService.GetRecentAsync(
            periodResolver.Resolve(request),
            page <= 0 ? 1 : page,
            pageSize <= 0 ? 20 : pageSize,
            managerId,
            status,
            ct));
}
