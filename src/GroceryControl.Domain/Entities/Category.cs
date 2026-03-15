using GroceryControl.Domain.Enums;

namespace GroceryControl.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public UnitCategory DefaultUnitCategory { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
