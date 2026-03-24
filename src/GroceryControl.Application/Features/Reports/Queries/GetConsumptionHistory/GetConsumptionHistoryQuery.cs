using MediatR;

namespace GroceryControl.Application.Features.Reports.Queries.GetConsumptionHistory;

public record GetConsumptionHistoryQuery(Guid ProductId) : IRequest<ConsumptionHistoryDto>;

public class ConsumptionHistoryDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public List<ConsumptionDataPoint> WeeklyConsumption { get; set; } = new();
    public List<ConsumptionDataPoint> MonthlyConsumption { get; set; } = new();
    public decimal AverageWeeklyConsumption { get; set; }
    public decimal AverageMonthlyConsumption { get; set; }
}

public class ConsumptionDataPoint
{
    public string Period { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal TotalSpent { get; set; }
}
