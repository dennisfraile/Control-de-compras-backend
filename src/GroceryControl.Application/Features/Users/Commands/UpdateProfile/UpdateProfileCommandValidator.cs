using FluentValidation;

namespace GroceryControl.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.HouseholdSize)
            .GreaterThanOrEqualTo(1).WithMessage("Household size must be at least 1.")
            .LessThanOrEqualTo(50).WithMessage("Household size must not exceed 50.");

        RuleFor(x => x.PreferredCurrency)
            .NotEmpty().WithMessage("Preferred currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code.");

        RuleFor(x => x.PurchaseFrequency)
            .IsInEnum().WithMessage("Invalid purchase frequency.");
    }
}
