namespace GroceryControl.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? Brand,
    string? Barcode,
    int CategoryId,
    string CategoryName,
    int DefaultUnitTypeId,
    string UnitAbbreviation,
    decimal DefaultQuantity,
    string? ImageUrl,
    string? Notes,
    decimal PackageSize,
    string? PackageLabel,
    bool IsGlobal,
    Guid? CreatedByUserId,
    DateTime CreatedAtUtc);
