using GroceryControl.Application.Features.Reports.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Reports.Queries.GetPeriodSummary;

public record GetPeriodSummaryQuery(string Period) : IRequest<PeriodSummaryDto>;
