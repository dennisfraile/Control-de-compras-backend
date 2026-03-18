namespace GroceryControl.Application.Common.Interfaces;

public interface IConsumptionAnalyzer
{
    Task<List<RestockAlert>> GetRestockAlertsAsync(Guid userId, CancellationToken ct);
}

public enum UrgencyLevel
{
    Critical,
    Warning,
    Info
}

public record RestockAlert(
    Guid ProductId,
    string ProductName,
    decimal CurrentStock,
    decimal DailyConsumptionRate,
    decimal EstimatedDaysRemaining,
    UrgencyLevel UrgencyLevel,
    decimal RecommendedQuantity);
