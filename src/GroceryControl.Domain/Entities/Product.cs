namespace GroceryControl.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public int DefaultUnitTypeId { get; set; }
    public decimal DefaultQuantity { get; set; } = 1;
    public string? ImageUrl { get; set; }
    public bool IsGlobal { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;
    public UnitType DefaultUnitType { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<InventoryEntry> InventoryEntries { get; set; } = new List<InventoryEntry>();
    public ICollection<PriceSuggestion> PriceSuggestions { get; set; } = new List<PriceSuggestion>();
}
