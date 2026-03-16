using GroceryControl.Application.Features.Purchases.Commands.CreatePurchase;
using GroceryControl.Application.Features.Purchases.Commands.DeletePurchase;
using GroceryControl.Application.Features.Purchases.DTOs;
using GroceryControl.Application.Features.Purchases.Queries.ExportPurchases;
using GroceryControl.Application.Features.Purchases.Queries.GetPurchases;
using GroceryControl.Application.Features.Purchases.Queries.GetPurchaseSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PurchaseDto>>> GetAll(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPurchasesQuery(from, to, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> Create(
        [FromBody] CreatePurchaseCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeletePurchaseCommand(id), ct);
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var bytes = await _mediator.Send(new ExportPurchasesQuery(from, to), ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "compras.xlsx");
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<PurchaseSummaryDto>>> GetSummary(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPurchaseSummaryQuery(year, month), ct);
        return Ok(result);
    }
}
