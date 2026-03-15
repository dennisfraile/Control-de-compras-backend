using GroceryControl.Application.Features.Users.Commands.UpdateProfile;
using GroceryControl.Application.Features.Users.DTOs;
using GroceryControl.Application.Features.Users.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileQuery(), ct);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UpdateProfileCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
