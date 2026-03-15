using GroceryControl.Application.Features.Users.DTOs;

namespace GroceryControl.Application.Features.Auth.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? PictureUrl);
