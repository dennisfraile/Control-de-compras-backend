namespace GroceryControl.Application.Features.Inventory.DTOs;

public record InventoryEntryDto(
    Guid Id,
    Guid UserId,
    Guid ProductId,
    string ProductName,
    string? ProductBrand,
    decimal CurrentQuantity,
    int UnitTypeId,
    string UnitAbbreviation,
    decimal MinimumThreshold,
    DateTime LastUpdatedUtc);
