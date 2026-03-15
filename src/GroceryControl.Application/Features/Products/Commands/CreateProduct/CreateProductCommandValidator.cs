using FluentValidation;

namespace GroceryControl.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand must not exceed 100 characters.");

        RuleFor(x => x.Barcode)
            .MaximumLength(50).WithMessage("Barcode must not exceed 50 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category is required.");

        RuleFor(x => x.DefaultUnitTypeId)
            .GreaterThan(0).WithMessage("A valid unit type is required.");

        RuleFor(x => x.DefaultQuantity)
            .GreaterThan(0).WithMessage("Default quantity must be greater than 0.");
    }
}
