using GroceryControl.Domain.Enums;

namespace GroceryControl.Application.Features.Users.DTOs;

public record UserProfileDto(
    Guid Id,
    Guid UserId,
    PurchaseFrequency PurchaseFrequency,
    int HouseholdSize,
    string PreferredCurrency,
    DateTime UpdatedAtUtc);
