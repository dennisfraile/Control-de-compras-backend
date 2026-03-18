using GroceryControl.Application.Features.Recipes.DTOs;
using GroceryControl.Application.Features.Recipes.Queries.GetRecipeSuggestions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<List<RecipeSuggestionDto>>> GetSuggestions(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRecipeSuggestionsQuery(), ct);
        return Ok(result);
    }
}
