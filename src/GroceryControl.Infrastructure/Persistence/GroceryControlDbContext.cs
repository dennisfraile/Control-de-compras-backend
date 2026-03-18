using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using GroceryControl.Infrastructure.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Persistence;

public class GroceryControlDbContext : DbContext, IApplicationDbContext
{
    public GroceryControlDbContext(DbContextOptions<GroceryControlDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<InventoryEntry> InventoryEntries => Set<InventoryEntry>();
    public DbSet<PriceSuggestion> PriceSuggestions => Set<PriceSuggestion>();
    public DbSet<UserFavoriteProduct> UserFavoriteProducts => Set<UserFavoriteProduct>();
    public DbSet<ShoppingTemplate> ShoppingTemplates => Set<ShoppingTemplate>();
    public DbSet<ShoppingTemplateItem> ShoppingTemplateItems => Set<ShoppingTemplateItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroceryControlDbContext).Assembly);

        CategorySeed.Seed(modelBuilder);
        UnitTypeSeed.Seed(modelBuilder);
    }
}
