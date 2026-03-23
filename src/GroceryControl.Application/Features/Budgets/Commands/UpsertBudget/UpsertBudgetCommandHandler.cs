using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Budgets.DTOs;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Budgets.Commands.UpsertBudget;

public class UpsertBudgetCommandHandler : IRequestHandler<UpsertBudgetCommand, BudgetStatusDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpsertBudgetCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BudgetStatusDto> Handle(UpsertBudgetCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var now = DateTime.UtcNow;

        var budget = await _context.PurchaseBudgets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.UpdatedAtUtc)
            .FirstOrDefaultAsync(ct);

        if (budget is null)
        {
            budget = new PurchaseBudget
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                Period = request.Period,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            _context.PurchaseBudgets.Add(budget);
        }
        else
        {
            budget.Amount = request.Amount;
            budget.Period = request.Period;
            budget.UpdatedAtUtc = now;
        }

        await _context.SaveChangesAsync(ct);

        var periodStart = budget.Period switch
        {
            "weekly" => now.AddDays(-7),
            "biweekly" => now.AddDays(-14),
            _ => now.AddDays(-30)
        };

        var spent = await _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.PurchaseDateUtc >= periodStart)
            .SumAsync(p => p.TotalAmount, ct);

        var remaining = Math.Max(0, budget.Amount - spent);
        var percentUsed = budget.Amount > 0
            ? Math.Round(spent / budget.Amount * 100, 1)
            : 0;

        return new BudgetStatusDto(
            budget.Id,
            budget.Amount,
            budget.Period,
            spent,
            remaining,
            percentUsed);
    }
}
