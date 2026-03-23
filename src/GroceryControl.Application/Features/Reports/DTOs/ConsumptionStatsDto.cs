namespace GroceryControl.Application.Features.Reports.DTOs;

public record ConsumptionStatsDto(
    TopItemDto? MostPurchasedProduct,
    TopItemDto? MostVisitedStore,
    TopSpendingCategoryDto? TopCategory,
    decimal AverageSpendingPerPurchase,
    List<MonthlyTrendDto> MonthlyTrend);

public record TopItemDto(string Name, int Count);
public record TopSpendingCategoryDto(string Name, decimal Total);
public record MonthlyTrendDto(string Month, decimal Total);
