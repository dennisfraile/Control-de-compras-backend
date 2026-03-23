using GroceryControl.Application.Features.Budgets.Commands.UpsertBudget;
using GroceryControl.Application.Features.Budgets.DTOs;
using GroceryControl.Application.Features.Budgets.Queries.GetCurrentBudget;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BudgetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("current")]
    public async Task<ActionResult<BudgetStatusDto>> GetCurrent(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCurrentBudgetQuery(), ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<BudgetStatusDto>> Upsert(
        [FromBody] UpsertBudgetCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
