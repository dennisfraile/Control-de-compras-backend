using GroceryControl.Domain.Entities;

namespace GroceryControl.Application.Common.Interfaces;

public record AuthResult(User User, bool IsNewUser);

public interface IGoogleAuthService
{
    Task<AuthResult> ValidateGoogleTokenAsync(string idToken, CancellationToken ct = default);
}
