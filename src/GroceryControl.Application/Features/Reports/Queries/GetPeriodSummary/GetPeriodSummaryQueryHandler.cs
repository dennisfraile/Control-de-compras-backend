using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Reports.Queries.GetPeriodSummary;

public class GetPeriodSummaryQueryHandler : IRequestHandler<GetPeriodSummaryQuery, PeriodSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPeriodSummaryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PeriodSummaryDto> Handle(GetPeriodSummaryQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var days = request.Period == "week" ? 7 : 30;
        var periodStart = now.AddDays(-days);
        var prevPeriodStart = periodStart.AddDays(-days);

        var userId = _currentUser.UserId;

        // Current period purchases
        var currentPurchases = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .Include(p => p.Store)
            .Where(p => p.UserId == userId && p.PurchaseDateUtc >= periodStart)
            .ToListAsync(ct);

        // Previous period
        var prevPurchases = await _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.PurchaseDateUtc >= prevPeriodStart && p.PurchaseDateUtc < periodStart)
            .ToListAsync(ct);

        var totalSpent = currentPurchases.Sum(p => p.TotalAmount);
        var prevSpent = prevPurchases.Sum(p => p.TotalAmount);
        var changePercent = prevSpent > 0 ? Math.Round((totalSpent - prevSpent) / prevSpent * 100, 1) : 0;

        // Top products
        var topProducts = currentPurchases
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Product.Name)
            .Select(g => new TopProductDto(g.Key, g.Sum(i => i.TotalPrice)))
            .OrderByDescending(x => x.TotalSpent)
            .Take(5)
            .ToList();

        // Top stores
        var topStores = currentPurchases
            .GroupBy(p => p.Store.Name)
            .Select(g => new TopStoreDto(g.Key, g.Sum(p => p.TotalAmount)))
            .OrderByDescending(x => x.TotalSpent)
            .Take(3)
            .ToList();

        // Low stock
        var lowStock = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Include(ie => ie.UnitType)
            .Where(ie => ie.UserId == userId && ie.CurrentQuantity <= ie.MinimumThreshold)
            .Select(ie => new LowStockItemDto(ie.Product.Name, ie.CurrentQuantity, ie.UnitType.Abbreviation))
            .ToListAsync(ct);

        // Expiring
        var expiryLimit = now.AddDays(7);
        var expiring = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Where(ie => ie.UserId == userId && ie.ExpirationDateUtc != null && ie.ExpirationDateUtc <= expiryLimit)
            .Select(ie => new ExpiringItemDto(
                ie.Product.Name,
                ie.ExpirationDateUtc!.Value,
                (int)(ie.ExpirationDateUtc!.Value - now).TotalDays))
            .ToListAsync(ct);

        return new PeriodSummaryDto(
            totalSpent, currentPurchases.Count, prevSpent, changePercent,
            topProducts, topStores, lowStock, expiring);
    }
}
