namespace GroceryControl.Application.Features.Purchases.DTOs;

public record PurchaseDto(
    Guid Id,
    Guid UserId,
    Guid StoreId,
    string StoreName,
    DateTime PurchaseDateUtc,
    decimal TotalAmount,
    string? Notes,
    DateTime CreatedAtUtc,
    List<PurchaseItemDto> Items);
