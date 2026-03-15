using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Persistence.Seeds;

public static class UnitTypeSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnitType>().HasData(
            new UnitType { Id = 1, Name = "Mililitros", Abbreviation = "ml", MeasurementUnit = MeasurementUnit.Milliliter, UnitCategory = UnitCategory.Liquid, ConversionFactorToBase = 1m },
            new UnitType { Id = 2, Name = "Litros", Abbreviation = "L", MeasurementUnit = MeasurementUnit.Liter, UnitCategory = UnitCategory.Liquid, ConversionFactorToBase = 1000m },
            new UnitType { Id = 3, Name = "Gramos", Abbreviation = "g", MeasurementUnit = MeasurementUnit.Gram, UnitCategory = UnitCategory.Grain, ConversionFactorToBase = 1m },
            new UnitType { Id = 4, Name = "Kilogramos", Abbreviation = "kg", MeasurementUnit = MeasurementUnit.Kilogram, UnitCategory = UnitCategory.Grain, ConversionFactorToBase = 1000m },
            new UnitType { Id = 5, Name = "Unidad", Abbreviation = "ud", MeasurementUnit = MeasurementUnit.Unit, UnitCategory = UnitCategory.Countable, ConversionFactorToBase = 1m },
            new UnitType { Id = 6, Name = "Pieza", Abbreviation = "pz", MeasurementUnit = MeasurementUnit.Piece, UnitCategory = UnitCategory.Countable, ConversionFactorToBase = 1m }
        );
    }
}
