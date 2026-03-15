using GroceryControl.Domain.Enums;

namespace GroceryControl.Domain.Entities;

public class UnitType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public MeasurementUnit MeasurementUnit { get; set; }
    public UnitCategory UnitCategory { get; set; }
    public decimal ConversionFactorToBase { get; set; } = 1;

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
