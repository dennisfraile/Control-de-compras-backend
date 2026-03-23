using MediatR;

namespace GroceryControl.Application.Features.ShoppingList.Queries.GetShareText;

public record GetShoppingListShareTextQuery : IRequest<string>;
