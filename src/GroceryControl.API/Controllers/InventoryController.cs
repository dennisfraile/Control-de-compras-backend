using GroceryControl.Application.Features.Inventory.Commands.ConsumeStock;
using GroceryControl.Application.Features.Inventory.Commands.UpdateStock;
using GroceryControl.Application.Features.Inventory.DTOs;
using GroceryControl.Application.Features.Inventory.Queries.ExportInventory;
using GroceryControl.Application.Features.Inventory.Queries.GetInventory;
using GroceryControl.Application.Features.Inventory.Queries.GetLowStock;
using GroceryControl.Application.Features.Inventory.Queries.GetExpiringItems;
using GroceryControl.Application.Features.Inventory.Queries.GetRestockAlerts;
using GroceryControl.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.API.Controllers;

public record QuickConsumeRequest(decimal Quantity = 1);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InventoryController(IMediator mediator, IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<List<InventoryEntryDto>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetInventoryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(CancellationToken ct)
    {
        var bytes = await _mediator.Send(new ExportInventoryQuery(), ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "inventario.xlsx");
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<InventoryEntryDto>>> GetLowStock(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLowStockQuery(), ct);
        return Ok(result);
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<List<ExpiringItemDto>>> GetExpiring(
        [FromQuery] int daysAhead = 7, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetExpiringItemsQuery(daysAhead), ct);
        return Ok(result);
    }

    [HttpGet("restock-alerts")]
    public async Task<ActionResult<List<RestockAlert>>> GetRestockAlerts(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRestockAlertsQuery(), ct);
        return Ok(result);
    }

    [HttpPut("{productId:guid}")]
    public async Task<ActionResult<InventoryEntryDto>> UpdateStock(
        Guid productId, [FromBody] UpdateStockCommand command, CancellationToken ct)
    {
        if (productId != command.ProductId) return BadRequest();
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPatch("{productId:guid}/quick-consume")]
    public async Task<IActionResult> QuickConsume(
        Guid productId, [FromBody] QuickConsumeRequest? request = null, CancellationToken ct = default)
    {
        var qty = request?.Quantity ?? 1;
        var entry = await _context.InventoryEntries
            .FirstOrDefaultAsync(ie => ie.UserId == _currentUser.UserId && ie.ProductId == productId, ct);
        if (entry == null) return NotFound();

        entry.CurrentQuantity = Math.Max(0, entry.CurrentQuantity - qty);
        entry.LastUpdatedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        return Ok(entry);
    }

    [HttpPatch("{productId:guid}/consume")]
    public async Task<ActionResult<InventoryEntryDto>> ConsumeStock(
        Guid productId, [FromBody] ConsumeStockCommand command, CancellationToken ct)
    {
        if (productId != command.ProductId) return BadRequest();
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
