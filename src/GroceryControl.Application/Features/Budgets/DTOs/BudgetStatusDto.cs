namespace GroceryControl.Application.Features.Budgets.DTOs;

public record BudgetStatusDto(
    Guid? Id,
    decimal BudgetAmount,
    string Period,
    decimal SpentThisPeriod,
    decimal Remaining,
    decimal PercentUsed);
