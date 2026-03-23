using GroceryControl.Application.Features.Households.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Households.Commands.InviteMember;

public record InviteMemberCommand(Guid HouseholdId, string Email) : IRequest<HouseholdMemberDto>;
