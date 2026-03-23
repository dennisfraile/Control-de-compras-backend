using GroceryControl.Application.Features.Budgets.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Budgets.Queries.GetCurrentBudget;

public record GetCurrentBudgetQuery() : IRequest<BudgetStatusDto>;
