using GroceryControl.Application.Features.Households.Commands.CreateHousehold;
using GroceryControl.Application.Features.Households.Commands.InviteMember;
using GroceryControl.Application.Features.Households.DTOs;
using GroceryControl.Application.Features.Households.Queries.GetMyHousehold;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseholdsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HouseholdsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<HouseholdDto>> Create(
        [FromBody] CreateHouseholdCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetMine), new { }, result);
    }

    [HttpGet("mine")]
    public async Task<ActionResult<HouseholdDto>> GetMine(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyHouseholdQuery(), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("{id:guid}/invite")]
    public async Task<ActionResult<HouseholdMemberDto>> Invite(
        Guid id, [FromBody] InviteRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new InviteMemberCommand(id, request.Email), ct);
        return Ok(result);
    }
}

public record InviteRequest(string Email);
