using GroceryControl.Application.Features.PriceSuggestions.Commands.SubmitPrice;
using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using GroceryControl.Application.Features.PriceSuggestions.Queries.GetBestDeals;
using GroceryControl.Application.Features.PriceSuggestions.Queries.GetLowestPrices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/price-suggestions")]
[Authorize]
public class PriceSuggestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PriceSuggestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PriceSuggestionDto>>> GetLowestPrices(
        [FromQuery] Guid? productId, [FromQuery] Guid? storeId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLowestPricesQuery(productId ?? Guid.Empty), ct);
        return Ok(result);
    }

    [HttpGet("best-deals")]
    public async Task<ActionResult<List<PriceSuggestionDto>>> GetBestDeals(
        [FromQuery] int? categoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBestDealsQuery(categoryId), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PriceSuggestionDto>> Submit(
        [FromBody] SubmitPriceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetLowestPrices), new { }, result);
    }
}
