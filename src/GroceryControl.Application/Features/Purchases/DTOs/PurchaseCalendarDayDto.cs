namespace GroceryControl.Application.Features.Purchases.DTOs;

public record PurchaseCalendarDayDto(
    DateTime Date,
    int PurchaseCount,
    decimal TotalSpent,
    List<string> StoreNames
);
