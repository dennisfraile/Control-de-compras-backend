using FluentValidation;

namespace GroceryControl.Application.Features.Stores.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(200).WithMessage("Store name must not exceed 200 characters.");

        RuleFor(x => x.Branch)
            .MaximumLength(100).WithMessage("Branch must not exceed 100 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");
    }
}
