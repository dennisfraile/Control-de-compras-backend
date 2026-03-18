namespace GroceryControl.Domain.Entities;

public class ShoppingTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public ICollection<ShoppingTemplateItem> Items { get; set; } = new List<ShoppingTemplateItem>();
}
