using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.ShoppingList.DTOs;
using GroceryControl.Domain.Enums;
using GroceryControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Services;

public class ShoppingListGenerator : IShoppingListGenerator
{
    private readonly GroceryControlDbContext _context;

    public ShoppingListGenerator(GroceryControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShoppingListSuggestionDto>> GenerateAsync(Guid userId, CancellationToken ct = default)
    {
        var suggestions = new List<ShoppingListSuggestionDto>();

        // Get user profile for purchase frequency
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        var cycleDays = profile is not null ? (int)profile.PurchaseFrequency : (int)PurchaseFrequency.Monthly;

        // Get user's inventory entries with related data
        var inventory = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Include(ie => ie.UnitType)
            .Where(ie => ie.UserId == userId)
            .ToListAsync(ct);

        // Get purchase history from the last 90 days for consumption analysis
        var sinceDate = DateTime.UtcNow.AddDays(-90);

        var purchaseItems = await _context.PurchaseItems
            .AsNoTracking()
            .Include(pi => pi.Product)
            .Include(pi => pi.UnitType)
            .Include(pi => pi.Purchase)
            .Where(pi => pi.Purchase.UserId == userId && pi.Purchase.PurchaseDateUtc >= sinceDate)
            .ToListAsync(ct);

        // Calculate average consumption per product per cycle
        var daysCovered = (DateTime.UtcNow - sinceDate).TotalDays;
        var consumptionByProduct = purchaseItems
            .GroupBy(pi => pi.ProductId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var totalQuantity = g.Sum(pi => NormalizeToBaseUnit(pi.Quantity, pi.UnitType.ConversionFactorToBase));
                    var avgPerDay = totalQuantity / (decimal)daysCovered;
                    return avgPerDay * cycleDays;
                });

        // Get product names/info for products in purchase history that are not in inventory
        var inventoryProductIds = inventory.Select(ie => ie.ProductId).ToHashSet();
        var historyOnlyProductIds = consumptionByProduct.Keys
            .Where(pid => !inventoryProductIds.Contains(pid))
            .ToList();

        var historyOnlyProducts = await _context.Products
            .AsNoTracking()
            .Include(p => p.DefaultUnitType)
            .Where(p => historyOnlyProductIds.Contains(p.Id))
            .ToListAsync(ct);

        // Check inventory items: below threshold or below 50% of average consumption
        foreach (var entry in inventory)
        {
            var currentStockBase = NormalizeToBaseUnit(entry.CurrentQuantity, entry.UnitType.ConversionFactorToBase);
            var thresholdBase = NormalizeToBaseUnit(entry.MinimumThreshold, entry.UnitType.ConversionFactorToBase);

            string? reason = null;
            decimal suggestedQuantity = 0;

            if (currentStockBase <= thresholdBase)
            {
                reason = "Stock por debajo del umbral mínimo";
                suggestedQuantity = consumptionByProduct.TryGetValue(entry.ProductId, out var avgConsumption)
                    ? avgConsumption
                    : entry.MinimumThreshold * 2;
            }
            else if (consumptionByProduct.TryGetValue(entry.ProductId, out var avgConsumption) && avgConsumption > 0)
            {
                var halfAvg = avgConsumption * 0.5m;
                if (currentStockBase < halfAvg)
                {
                    reason = "Stock por debajo del 50% del consumo promedio por ciclo";
                    suggestedQuantity = avgConsumption;
                }
            }

            if (reason is null)
                continue;

            // Convert suggested quantity back to the entry's unit
            var suggestedInUnit = suggestedQuantity / entry.UnitType.ConversionFactorToBase;

            // Round up to whole packages
            var packageSize = entry.Product.PackageSize > 0 ? entry.Product.PackageSize : 1;
            var packages = Math.Ceiling(suggestedInUnit / packageSize);
            var roundedQuantity = packages * packageSize;

            // Get lowest known price
            var (lowestPrice, lowestStore) = await GetLowestPriceAsync(entry.ProductId, ct);

            suggestions.Add(new ShoppingListSuggestionDto(
                ProductId: entry.ProductId,
                ProductName: entry.Product.Name,
                SuggestedQuantity: roundedQuantity,
                UnitAbbreviation: entry.UnitType.Abbreviation,
                CurrentStock: Math.Round(entry.CurrentQuantity, 2),
                Reason: reason,
                LowestKnownPrice: lowestPrice,
                LowestPriceStore: lowestStore,
                PackageLabel: entry.Product.PackageLabel,
                PackageSize: packageSize));
        }

        // Also suggest frequently purchased products not currently in inventory
        foreach (var product in historyOnlyProducts)
        {
            var avgConsumption = consumptionByProduct[product.Id];
            if (avgConsumption <= 0)
                continue;

            var suggestedInUnit = avgConsumption / product.DefaultUnitType.ConversionFactorToBase;

            // Round up to whole packages
            var pkgSize = product.PackageSize > 0 ? product.PackageSize : 1;
            var pkgCount = Math.Ceiling(suggestedInUnit / pkgSize);
            var roundedQty = pkgCount * pkgSize;

            var (lowestPrice, lowestStore) = await GetLowestPriceAsync(product.Id, ct);

            suggestions.Add(new ShoppingListSuggestionDto(
                ProductId: product.Id,
                ProductName: product.Name,
                SuggestedQuantity: roundedQty,
                UnitAbbreviation: product.DefaultUnitType.Abbreviation,
                CurrentStock: 0,
                Reason: "Producto comprado frecuentemente sin stock registrado",
                LowestKnownPrice: lowestPrice,
                LowestPriceStore: lowestStore,
                PackageLabel: product.PackageLabel,
                PackageSize: pkgSize));
        }

        return suggestions;
    }

    private static decimal NormalizeToBaseUnit(decimal quantity, decimal conversionFactor)
    {
        return quantity * conversionFactor;
    }

    private async Task<(decimal? Price, string? StoreName)> GetLowestPriceAsync(Guid productId, CancellationToken ct)
    {
        var suggestion = await _context.PriceSuggestions
            .AsNoTracking()
            .Include(ps => ps.Store)
            .Where(ps => ps.ProductId == productId)
            .OrderBy(ps => ps.Price / ps.Quantity)
            .FirstOrDefaultAsync(ct);

        if (suggestion is null)
            return (null, null);

        return (suggestion.Price, suggestion.Store.Name);
    }
}
