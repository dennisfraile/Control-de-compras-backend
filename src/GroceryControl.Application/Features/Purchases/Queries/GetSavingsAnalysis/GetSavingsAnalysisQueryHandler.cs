using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.GetSavingsAnalysis;

public class GetSavingsAnalysisQueryHandler : IRequestHandler<GetSavingsAnalysisQuery, SavingsAnalysisDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSavingsAnalysisQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SavingsAnalysisDto> Handle(GetSavingsAnalysisQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        // Get purchases for current and last month
        var recentPurchases = await _context.Purchases
            .Include(p => p.Store)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Where(p => p.UserId == userId && p.PurchaseDateUtc >= lastMonthStart)
            .ToListAsync(ct);

        var thisMonthPurchases = recentPurchases.Where(p => p.PurchaseDateUtc >= thisMonthStart).ToList();
        var lastMonthPurchases = recentPurchases.Where(p => p.PurchaseDateUtc >= lastMonthStart && p.PurchaseDateUtc < thisMonthStart).ToList();

        var totalThisMonth = thisMonthPurchases.Sum(p => p.Items.Sum(i => i.Quantity * i.UnitPrice));
        var totalLastMonth = lastMonthPurchases.Sum(p => p.Items.Sum(i => i.Quantity * i.UnitPrice));

        var monthOverMonthChange = totalLastMonth > 0
            ? Math.Round((totalThisMonth - totalLastMonth) / totalLastMonth * 100, 2)
            : 0;

        // Store comparisons
        var storeComparisons = recentPurchases
            .Where(p => p.Store != null)
            .GroupBy(p => new { p.StoreId, p.Store!.Name })
            .Select(g =>
            {
                var items = g.SelectMany(p => p.Items).ToList();
                var totalSpent = items.Sum(i => i.Quantity * i.UnitPrice);
                var itemCount = items.Count;
                return new StoreSavingDto(
                    g.Key.StoreId,
                    g.Key.Name,
                    totalSpent,
                    itemCount > 0 ? Math.Round(totalSpent / itemCount, 2) : 0,
                    itemCount
                );
            })
            .OrderByDescending(s => s.TotalSpent)
            .ToList();

        // Product price comparison across stores
        var allItems = recentPurchases
            .Where(p => p.Store != null)
            .SelectMany(p => p.Items.Select(i => new
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Desconocido",
                StoreName = p.Store!.Name,
                UnitPrice = i.UnitPrice
            }))
            .ToList();

        var productSavings = allItems
            .GroupBy(i => new { i.ProductId, i.ProductName })
            .Where(g => g.Select(x => x.StoreName).Distinct().Count() > 1) // bought at 2+ stores
            .Select(g =>
            {
                var cheapest = g.OrderBy(x => x.UnitPrice).First();
                var mostExpensive = g.OrderByDescending(x => x.UnitPrice).First();
                return new ProductSavingDto(
                    g.Key.ProductId,
                    g.Key.ProductName,
                    cheapest.UnitPrice,
                    cheapest.StoreName,
                    mostExpensive.UnitPrice,
                    mostExpensive.StoreName,
                    mostExpensive.UnitPrice - cheapest.UnitPrice
                );
            })
            .Where(p => p.PotentialSavingPerUnit > 0)
            .OrderByDescending(p => p.PotentialSavingPerUnit)
            .Take(20)
            .ToList();

        var potentialSavings = productSavings.Sum(p => p.PotentialSavingPerUnit);

        return new SavingsAnalysisDto(
            totalThisMonth,
            totalLastMonth,
            monthOverMonthChange,
            potentialSavings,
            storeComparisons,
            productSavings
        );
    }
}
