namespace GroceryControl.Application.Features.PriceSuggestions.DTOs;

public record PriceSuggestionDto(
    string ProductName,
    string StoreName,
    decimal Price,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime ObservedDate);
