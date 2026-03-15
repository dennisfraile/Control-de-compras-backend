namespace GroceryControl.Application.Features.Purchases.DTOs;

public record PurchaseSummaryDto(
    int Year,
    int Month,
    decimal TotalSpent,
    int TotalPurchases,
    int TotalItems);
