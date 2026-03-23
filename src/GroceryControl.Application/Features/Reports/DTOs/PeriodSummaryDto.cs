namespace GroceryControl.Application.Features.Reports.DTOs;

public record PeriodSummaryDto(
    decimal TotalSpent,
    int PurchaseCount,
    decimal PreviousPeriodSpent,
    decimal ChangePercent,
    List<TopProductDto> TopProducts,
    List<TopStoreDto> TopStores,
    List<LowStockItemDto> LowStockItems,
    List<ExpiringItemDto> ExpiringItems
);

public record TopProductDto(string ProductName, decimal TotalSpent);
public record TopStoreDto(string StoreName, decimal TotalSpent);
public record LowStockItemDto(string ProductName, decimal CurrentQuantity, string UnitAbbreviation);
public record ExpiringItemDto(string ProductName, DateTime ExpirationDate, int DaysUntilExpiry);
