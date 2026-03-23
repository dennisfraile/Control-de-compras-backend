using GroceryControl.Application.Features.Reports.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Reports.Queries.GetConsumptionStats;

public record GetConsumptionStatsQuery() : IRequest<ConsumptionStatsDto>;
