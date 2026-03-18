namespace GroceryControl.Application.Features.Purchases.DTOs;

public record SavingsAnalysisDto(
    decimal TotalSpentThisMonth,
    decimal TotalSpentLastMonth,
    decimal MonthOverMonthChange,
    decimal PotentialSavings,
    List<StoreSavingDto> StoreComparisons,
    List<ProductSavingDto> TopSavings
);

public record StoreSavingDto(
    Guid StoreId,
    string StoreName,
    decimal TotalSpent,
    decimal AveragePrice,
    int ItemCount
);

public record ProductSavingDto(
    Guid ProductId,
    string ProductName,
    decimal CheapestPrice,
    string CheapestStore,
    decimal MostExpensivePrice,
    string MostExpensiveStore,
    decimal PotentialSavingPerUnit
);
