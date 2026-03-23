using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Reports.Queries.GetConsumptionStats;

public class GetConsumptionStatsQueryHandler : IRequestHandler<GetConsumptionStatsQuery, ConsumptionStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetConsumptionStatsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ConsumptionStatsDto> Handle(GetConsumptionStatsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var purchases = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Items).ThenInclude(i => i.Product).ThenInclude(pr => pr.Category)
            .Include(p => p.Store)
            .Where(p => p.UserId == userId)
            .ToListAsync(ct);

        // Most purchased product (by count of purchase items)
        var mostPurchased = purchases
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Product.Name)
            .Select(g => new TopItemDto(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        // Most visited store
        var mostVisited = purchases
            .GroupBy(p => p.Store.Name)
            .Select(g => new TopItemDto(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        // Category with most spending
        var topCategory = purchases
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Product.Category.Name)
            .Select(g => new TopSpendingCategoryDto(g.Key, g.Sum(i => i.TotalPrice)))
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        // Average spending per purchase
        var avgSpending = purchases.Count > 0
            ? Math.Round(purchases.Average(p => p.TotalAmount), 2)
            : 0;

        // Monthly spending trend (last 6 months)
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var monthlyTrend = purchases
            .Where(p => p.PurchaseDateUtc >= sixMonthsAgo)
            .GroupBy(p => new { p.PurchaseDateUtc.Year, p.PurchaseDateUtc.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyTrendDto(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Sum(p => p.TotalAmount)))
            .ToList();

        return new ConsumptionStatsDto(
            mostPurchased,
            mostVisited,
            topCategory,
            avgSpending,
            monthlyTrend);
    }
}
