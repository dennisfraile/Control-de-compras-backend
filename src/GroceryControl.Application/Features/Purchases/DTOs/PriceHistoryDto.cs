namespace GroceryControl.Application.Features.Purchases.DTOs;

public record PriceHistoryDto(
    DateTime PurchaseDateUtc,
    string StoreName,
    decimal UnitPrice,
    decimal Quantity,
    string UnitAbbreviation);
