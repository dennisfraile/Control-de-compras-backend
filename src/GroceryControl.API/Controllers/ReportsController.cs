using GroceryControl.Application.Features.Reports.DTOs;
using GroceryControl.Application.Features.Reports.Queries.GetPeriodSummary;
using GroceryControl.Application.Features.Reports.Queries.GetConsumptionStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<PeriodSummaryDto>> GetSummary(
        [FromQuery] string period = "month", CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPeriodSummaryQuery(period), ct);
        return Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<ConsumptionStatsDto>> GetStatistics(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetConsumptionStatsQuery(), ct);
        return Ok(result);
    }
}
