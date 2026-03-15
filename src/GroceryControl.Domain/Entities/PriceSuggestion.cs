namespace GroceryControl.Domain.Entities;

public class PriceSuggestion
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public int UnitTypeId { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public DateTime ObservedDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Navigation (NO User navigation - enforces anonymity)
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
