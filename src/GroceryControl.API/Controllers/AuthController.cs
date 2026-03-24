using GroceryControl.Application.Features.Auth.Commands.GoogleLogin;
using GroceryControl.Application.Features.Auth.Commands.RefreshToken;
using GroceryControl.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin(
        [FromBody] GoogleLoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        [FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
