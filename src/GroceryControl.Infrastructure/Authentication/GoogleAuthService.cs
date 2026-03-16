using Google.Apis.Auth;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using GroceryControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GroceryControl.Infrastructure.Authentication;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GroceryControlDbContext _context;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(GroceryControlDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResult> ValidateGoogleTokenAsync(string idToken, CancellationToken ct = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _configuration["Google:ClientId"] ?? string.Empty }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject, ct);

        if (user is not null)
        {
            user.Email = payload.Email;
            user.DisplayName = payload.Name;
            user.PictureUrl = payload.Picture;
            user.UpdatedAtUtc = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);

            return new AuthResult(user, IsNewUser: false);
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            GoogleId = payload.Subject,
            Email = payload.Email,
            DisplayName = payload.Name,
            PictureUrl = payload.Picture,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            Profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UpdatedAtUtc = DateTime.UtcNow
            }
        };

        await _context.Users.AddAsync(newUser, ct);
        await _context.SaveChangesAsync(ct);

        return new AuthResult(newUser, IsNewUser: true);
    }
}
