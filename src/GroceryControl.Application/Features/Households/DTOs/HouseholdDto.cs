namespace GroceryControl.Application.Features.Households.DTOs;

public record HouseholdDto(
    Guid Id,
    string Name,
    Guid OwnerUserId,
    DateTime CreatedAtUtc,
    List<HouseholdMemberDto> Members);

public record HouseholdMemberDto(
    Guid Id,
    Guid UserId,
    string Email,
    string DisplayName,
    string Role,
    DateTime JoinedAtUtc);
