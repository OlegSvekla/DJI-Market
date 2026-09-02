using DJI.Bl.Services;
using DJI.Contracts.Rss;
using Microsoft.AspNetCore.Mvc;

namespace DJI.Web.Controllers;

[ApiController]
[Route("api/meta")]
public class MetaController(IFiltersService filtersService) : ControllerBase
{
    [HttpGet("filters")]
    [ProducesResponseType(typeof(FiltersRs), StatusCodes.Status200OK)]
    public async Task<ActionResult<FiltersRs>> GetFilters(CancellationToken ct)
        => Ok(await filtersService.GetAsync(ct));
}
