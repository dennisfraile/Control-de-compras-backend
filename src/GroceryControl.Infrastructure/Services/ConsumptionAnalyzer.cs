using GroceryControl.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Services;

public class ConsumptionAnalyzer : IConsumptionAnalyzer
{
    private readonly IApplicationDbContext _context;

    private const int LookbackDays = 90;
    private const int ShoppingCycleDays = 14;

    public ConsumptionAnalyzer(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RestockAlert>> GetRestockAlertsAsync(Guid userId, CancellationToken ct)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-LookbackDays);

        var inventoryEntries = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Where(ie => ie.UserId == userId)
            .ToListAsync(ct);

        if (inventoryEntries.Count == 0)
            return [];

        var productIds = inventoryEntries.Select(ie => ie.ProductId).ToList();

        var purchaseData = await _context.PurchaseItems
            .AsNoTracking()
            .Include(pi => pi.Purchase)
            .Where(pi => productIds.Contains(pi.ProductId)
                && pi.Purchase.UserId == userId
                && pi.Purchase.PurchaseDateUtc >= cutoffDate)
            .GroupBy(pi => pi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantityPurchased = g.Sum(pi => pi.Quantity),
                FirstPurchaseDate = g.Min(pi => pi.Purchase.PurchaseDateUtc),
                LastPurchaseDate = g.Max(pi => pi.Purchase.PurchaseDateUtc),
                PurchaseCount = g.Count()
            })
            .ToListAsync(ct);

        var purchaseLookup = purchaseData.ToDictionary(p => p.ProductId);

        var alerts = new List<RestockAlert>();

        foreach (var entry in inventoryEntries)
        {
            if (!purchaseLookup.TryGetValue(entry.ProductId, out var history))
                continue;

            var daysSinceFirstPurchase = (DateTime.UtcNow - history.FirstPurchaseDate).TotalDays;
            if (daysSinceFirstPurchase < 1)
                daysSinceFirstPurchase = 1;

            var dailyConsumptionRate = history.TotalQuantityPurchased / (decimal)daysSinceFirstPurchase;
            if (dailyConsumptionRate <= 0)
                continue;

            var estimatedDaysRemaining = entry.CurrentQuantity / dailyConsumptionRate;

            UrgencyLevel urgency;
            if (estimatedDaysRemaining < 3)
                urgency = UrgencyLevel.Critical;
            else if (estimatedDaysRemaining < 7)
                urgency = UrgencyLevel.Warning;
            else if (estimatedDaysRemaining < 14)
                urgency = UrgencyLevel.Info;
            else
                continue;

            var recommendedQuantity = Math.Ceiling(dailyConsumptionRate * ShoppingCycleDays);

            alerts.Add(new RestockAlert(
                entry.ProductId,
                entry.Product.Name,
                entry.CurrentQuantity,
                Math.Round(dailyConsumptionRate, 2),
                Math.Round(estimatedDaysRemaining, 1),
                urgency,
                recommendedQuantity));
        }

        return alerts
            .OrderBy(a => a.UrgencyLevel)
            .ThenBy(a => a.EstimatedDaysRemaining)
            .ToList();
    }
}
