namespace GroceryControl.Application.Features.Stores.DTOs;

public record StoreDto(
    Guid Id,
    string Name,
    string? Branch,
    string? Address,
    string? City,
    bool IsGlobal,
    Guid? CreatedByUserId,
    DateTime CreatedAtUtc);
