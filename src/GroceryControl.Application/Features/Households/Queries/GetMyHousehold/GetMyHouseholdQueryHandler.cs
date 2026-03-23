using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Households.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Households.Queries.GetMyHousehold;

public class GetMyHouseholdQueryHandler : IRequestHandler<GetMyHouseholdQuery, HouseholdDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyHouseholdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<HouseholdDto?> Handle(GetMyHouseholdQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var membership = await _context.HouseholdMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (membership is null)
            return null;

        var household = await _context.Households
            .AsNoTracking()
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == membership.HouseholdId, ct);

        if (household is null)
            return null;

        var userIds = household.Members.Select(m => m.UserId).ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        var members = household.Members.Select(m =>
        {
            var u = users[m.UserId];
            return new HouseholdMemberDto(m.Id, m.UserId, u.Email, u.DisplayName, m.Role, m.JoinedAtUtc);
        }).ToList();

        return new HouseholdDto(
            household.Id,
            household.Name,
            household.OwnerUserId,
            household.CreatedAtUtc,
            members);
    }
}
