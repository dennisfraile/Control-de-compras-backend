using GroceryControl.Application.Features.ShoppingList.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.ShoppingList.Queries.GenerateShoppingList;

public record GenerateShoppingListQuery : IRequest<List<ShoppingListSuggestionDto>>;
