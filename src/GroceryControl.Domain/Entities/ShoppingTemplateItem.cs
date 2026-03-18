namespace GroceryControl.Domain.Entities;

public class ShoppingTemplateItem
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public int UnitTypeId { get; set; }

    public ShoppingTemplate Template { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
