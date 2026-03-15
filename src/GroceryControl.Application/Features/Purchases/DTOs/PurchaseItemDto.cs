namespace GroceryControl.Application.Features.Purchases.DTOs;

public record PurchaseItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductBrand,
    decimal Quantity,
    int UnitTypeId,
    string UnitAbbreviation,
    decimal UnitPrice,
    decimal TotalPrice,
    bool AddToInventory);
