using GroceryControl.Application.Features.ShoppingList.DTOs;
using GroceryControl.Application.Features.ShoppingList.Queries.GenerateShoppingList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/shopping-list")]
[Authorize]
public class ShoppingListController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingListController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShoppingListSuggestionDto>>> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerateShoppingListQuery(), ct);
        return Ok(result);
    }

    [HttpGet("generate")]
    public async Task<ActionResult<List<ShoppingListSuggestionDto>>> Generate(CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerateShoppingListQuery(), ct);
        return Ok(result);
    }
}
