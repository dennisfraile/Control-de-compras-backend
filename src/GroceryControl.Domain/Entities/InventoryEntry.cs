namespace GroceryControl.Domain.Entities;

public class InventoryEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal CurrentQuantity { get; set; }
    public int UnitTypeId { get; set; }
    public decimal MinimumThreshold { get; set; }
    public DateTime LastUpdatedUtc { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
