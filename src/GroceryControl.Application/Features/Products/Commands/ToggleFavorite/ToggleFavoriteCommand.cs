using MediatR;

namespace GroceryControl.Application.Features.Products.Commands.ToggleFavorite;

public record ToggleFavoriteCommand(Guid ProductId) : IRequest<bool>;
