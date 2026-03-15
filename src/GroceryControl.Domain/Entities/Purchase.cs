namespace GroceryControl.Domain.Entities;

public class Purchase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoreId { get; set; }
    public DateTime PurchaseDateUtc { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
