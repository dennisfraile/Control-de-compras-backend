using GroceryControl.Application.Features.Budgets.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Budgets.Commands.UpsertBudget;

public record UpsertBudgetCommand(decimal Amount, string Period) : IRequest<BudgetStatusDto>;
