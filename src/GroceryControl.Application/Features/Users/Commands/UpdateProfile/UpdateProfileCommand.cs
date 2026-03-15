using GroceryControl.Application.Features.Users.DTOs;
using GroceryControl.Domain.Enums;
using MediatR;

namespace GroceryControl.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    PurchaseFrequency PurchaseFrequency,
    int HouseholdSize,
    string PreferredCurrency) : IRequest<UserProfileDto>;
