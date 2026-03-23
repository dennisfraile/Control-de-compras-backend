namespace GroceryControl.Application.Features.ShoppingList.DTOs;

public record ShoppingListSuggestionDto(
    Guid ProductId,
    string ProductName,
    decimal SuggestedQuantity,
    string UnitAbbreviation,
    decimal CurrentStock,
    string Reason,
    decimal? LowestKnownPrice,
    string? LowestPriceStore,
    string? PackageLabel = null,
    decimal PackageSize = 1);
