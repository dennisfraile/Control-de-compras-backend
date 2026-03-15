using System.Security.Claims;
using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Auth.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new ForbiddenException("Invalid access token.");

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new ForbiddenException("Invalid token claims.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new ForbiddenException("Invalid token claims.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new ForbiddenException("Invalid or expired refresh token.");

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAtUtc = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(user.Id, user.Email, user.DisplayName, user.PictureUrl);

        return new AuthResponseDto(newAccessToken, newRefreshToken, userDto);
    }
}
