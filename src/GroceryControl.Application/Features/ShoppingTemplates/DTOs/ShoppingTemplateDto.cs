namespace GroceryControl.Application.Features.ShoppingTemplates.DTOs;

public record ShoppingTemplateDto(
    Guid Id,
    string Name,
    DateTime CreatedAtUtc,
    List<ShoppingTemplateItemDto> Items
);

public record ShoppingTemplateItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    int UnitTypeId,
    string UnitAbbreviation
);
