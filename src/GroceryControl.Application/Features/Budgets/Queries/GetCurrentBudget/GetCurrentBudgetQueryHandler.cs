using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Budgets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Budgets.Queries.GetCurrentBudget;

public class GetCurrentBudgetQueryHandler : IRequestHandler<GetCurrentBudgetQuery, BudgetStatusDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentBudgetQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BudgetStatusDto> Handle(GetCurrentBudgetQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var budget = await _context.PurchaseBudgets
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.UpdatedAtUtc)
            .FirstOrDefaultAsync(ct);

        if (budget is null)
            return new BudgetStatusDto(null, 0, "monthly", 0, 0, 0);

        var now = DateTime.UtcNow;
        var periodStart = budget.Period switch
        {
            "weekly" => now.AddDays(-7),
            "biweekly" => now.AddDays(-14),
            _ => now.AddDays(-30) // monthly
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
