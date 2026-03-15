using GroceryControl.Application.Features.ShoppingList.DTOs;

namespace GroceryControl.Application.Common.Interfaces;

public interface IShoppingListGenerator
{
    Task<List<ShoppingListSuggestionDto>> GenerateAsync(Guid userId, CancellationToken ct = default);
}
