using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GroceryControl.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GroceryControl.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(userIdClaim, out var userId)
                ? userId
                : Guid.Empty;
        }
    }
}
