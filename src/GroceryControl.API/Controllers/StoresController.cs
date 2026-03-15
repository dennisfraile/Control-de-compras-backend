using GroceryControl.Application.Features.Stores.Commands.CreateStore;
using GroceryControl.Application.Features.Stores.DTOs;
using GroceryControl.Application.Features.Stores.Queries.GetStores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<StoreDto>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetStoresQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create(
        [FromBody] CreateStoreCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }
}
