using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Households.DTOs;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Households.Commands.CreateHousehold;

public class CreateHouseholdCommandHandler : IRequestHandler<CreateHouseholdCommand, HouseholdDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateHouseholdCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<HouseholdDto> Handle(CreateHouseholdCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var now = DateTime.UtcNow;

        var user = await _context.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == userId, ct);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerUserId = userId,
            CreatedAtUtc = now
        };

        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            UserId = userId,
            Role = "owner",
            JoinedAtUtc = now
        };

        _context.Households.Add(household);
        _context.HouseholdMembers.Add(member);
        await _context.SaveChangesAsync(ct);

        return new HouseholdDto(
            household.Id,
            household.Name,
            household.OwnerUserId,
            household.CreatedAtUtc,
            [new HouseholdMemberDto(member.Id, userId, user.Email, user.DisplayName, "owner", now)]);
    }
}
