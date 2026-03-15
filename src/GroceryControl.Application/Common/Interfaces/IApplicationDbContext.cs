using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Category> Categories { get; }
    DbSet<UnitType> UnitTypes { get; }
    DbSet<Product> Products { get; }
    DbSet<Store> Stores { get; }
    DbSet<Purchase> Purchases { get; }
    DbSet<PurchaseItem> PurchaseItems { get; }
    DbSet<InventoryEntry> InventoryEntries { get; }
    DbSet<PriceSuggestion> PriceSuggestions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
