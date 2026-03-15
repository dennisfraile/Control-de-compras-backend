using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Users.DTOs;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Users.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProfileQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(UserProfile), _currentUser.UserId);

        return new UserProfileDto(
            profile.Id,
            profile.UserId,
            profile.PurchaseFrequency,
            profile.HouseholdSize,
            profile.PreferredCurrency,
            profile.UpdatedAtUtc);
    }
}
