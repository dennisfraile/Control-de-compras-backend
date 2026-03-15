namespace GroceryControl.Domain.Entities;

public class PurchaseItem
{
    public Guid Id { get; set; }
    public Guid PurchaseId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public int UnitTypeId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool AddToInventory { get; set; } = true;

    // Navigation
    public Purchase Purchase { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
