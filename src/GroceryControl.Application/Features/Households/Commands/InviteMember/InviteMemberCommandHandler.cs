using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Households.DTOs;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Households.Commands.InviteMember;

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, HouseholdMemberDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InviteMemberCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<HouseholdMemberDto> Handle(InviteMemberCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var household = await _context.Households
            .FirstOrDefaultAsync(h => h.Id == request.HouseholdId, ct)
            ?? throw new NotFoundException(nameof(Household), request.HouseholdId);

        if (household.OwnerUserId != userId)
            throw new ForbiddenException("Only the household owner can invite members.");

        var invitedUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct)
            ?? throw new NotFoundException("User", request.Email);

        var alreadyMember = await _context.HouseholdMembers
            .AnyAsync(m => m.HouseholdId == request.HouseholdId && m.UserId == invitedUser.Id, ct);

        if (alreadyMember)
            throw new InvalidOperationException("User is already a member of this household.");

        var now = DateTime.UtcNow;
        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            UserId = invitedUser.Id,
            Role = "member",
            JoinedAtUtc = now
        };

        _context.HouseholdMembers.Add(member);
        await _context.SaveChangesAsync(ct);

        return new HouseholdMemberDto(
            member.Id,
            invitedUser.Id,
            invitedUser.Email,
            invitedUser.DisplayName,
            "member",
            now);
    }
}
