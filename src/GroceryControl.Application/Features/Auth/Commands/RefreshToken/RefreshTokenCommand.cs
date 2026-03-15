using GroceryControl.Application.Features.Auth.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponseDto>;
