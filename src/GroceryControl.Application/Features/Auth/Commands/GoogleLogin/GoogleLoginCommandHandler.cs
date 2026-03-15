using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Auth.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IJwtTokenService jwtTokenService,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _googleAuthService = googleAuthService;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
        var user = authResult.User;

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAtUtc = DateTime.UtcNow;

        if (authResult.IsNewUser)
        {
            await _userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            _userRepository.Update(user);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(user.Id, user.Email, user.DisplayName, user.PictureUrl);

        return new AuthResponseDto(accessToken, refreshToken, userDto);
    }
}
