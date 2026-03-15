using GroceryControl.Application.Features.Auth.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponseDto>;
