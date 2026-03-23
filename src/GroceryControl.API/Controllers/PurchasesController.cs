using GroceryControl.Application.Features.Purchases.Commands.CreatePurchase;
using GroceryControl.Application.Features.Purchases.Commands.DeletePurchase;
using GroceryControl.Application.Features.Purchases.Commands.QuickPurchase;
using GroceryControl.Application.Features.Purchases.Commands.ScanReceipt;
using GroceryControl.Application.Features.Purchases.DTOs;
using GroceryControl.Application.Features.Purchases.Queries.ExportPurchases;
using GroceryControl.Application.Features.Purchases.Queries.GetPurchaseCalendar;
using GroceryControl.Application.Features.Purchases.Queries.GetPurchases;
using GroceryControl.Application.Features.Purchases.Queries.GetPurchaseSummary;
using GroceryControl.Application.Features.Purchases.Queries.GetSavingsAnalysis;
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

    [HttpPost("quick")]
    public async Task<IActionResult> QuickPurchase(
        [FromBody] QuickPurchaseCommand command, CancellationToken ct)
    {
        var purchaseId = await _mediator.Send(command, ct);
        return Ok(new { id = purchaseId });
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

    [HttpPost("scan-receipt")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB max
    public async Task<IActionResult> ScanReceipt(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No se proporcionó imagen" });

        using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new ScanReceiptCommand(stream), ct);
        return Ok(result);
    }

    [HttpGet("savings-analysis")]
    public async Task<ActionResult<SavingsAnalysisDto>> GetSavingsAnalysis(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSavingsAnalysisQuery(), ct);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<PurchaseSummaryDto>>> GetSummary(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPurchaseSummaryQuery(year, month), ct);
        return Ok(result);
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<List<PurchaseCalendarDayDto>>> GetCalendar(
        [FromQuery] int year, [FromQuery] int month, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPurchaseCalendarQuery(year, month), ct);
        return Ok(result);
    }
}
