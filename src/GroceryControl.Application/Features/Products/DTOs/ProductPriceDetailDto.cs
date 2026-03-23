namespace GroceryControl.Application.Features.Products.DTOs;

public record ProductPriceDetailDto(
    string ProductName,
    string? ProductBrand,
    string CategoryName,
    decimal AveragePrice,
    decimal CheapestPrice,
    string CheapestStore,
    decimal MostExpensivePrice,
    string MostExpensiveStore,
    string Trend,
    List<PriceHistoryEntry> PriceHistory,
    decimal? PricePerBaseUnit = null,
    string? BaseUnitLabel = null
);

public record PriceHistoryEntry(
    DateTime Date,
    string StoreName,
    decimal UnitPrice,
    decimal Quantity
);
