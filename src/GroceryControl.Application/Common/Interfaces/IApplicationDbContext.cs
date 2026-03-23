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
    DbSet<UserFavoriteProduct> UserFavoriteProducts { get; }
    DbSet<ShoppingTemplate> ShoppingTemplates { get; }
    DbSet<ShoppingTemplateItem> ShoppingTemplateItems { get; }
    DbSet<PurchaseBudget> PurchaseBudgets { get; }
    DbSet<Household> Households { get; }
    DbSet<HouseholdMember> HouseholdMembers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
