using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Persistence.Seeds;

public static class CategorySeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Lácteos", Description = "Leche, queso, yogur y derivados lácteos", IconName = "milk", DefaultUnitCategory = UnitCategory.Liquid, SortOrder = 1 },
            new Category { Id = 2, Name = "Carnes", Description = "Carnes rojas, aves, cerdo y embutidos", IconName = "meat", DefaultUnitCategory = UnitCategory.Grain, SortOrder = 2 },
            new Category { Id = 3, Name = "Frutas y Verduras", Description = "Frutas frescas, verduras y hortalizas", IconName = "apple", DefaultUnitCategory = UnitCategory.Countable, SortOrder = 3 },
            new Category { Id = 4, Name = "Cereales y Granos", Description = "Arroz, avena, cereales y granos", IconName = "grain", DefaultUnitCategory = UnitCategory.Grain, SortOrder = 4 },
            new Category { Id = 5, Name = "Bebidas", Description = "Agua, jugos, refrescos y bebidas", IconName = "drink", DefaultUnitCategory = UnitCategory.Liquid, SortOrder = 5 },
            new Category { Id = 6, Name = "Limpieza", Description = "Productos de limpieza para el hogar", IconName = "cleaning", DefaultUnitCategory = UnitCategory.Liquid, SortOrder = 6 },
            new Category { Id = 7, Name = "Higiene Personal", Description = "Jabón, shampoo, pasta dental y cuidado personal", IconName = "hygiene", DefaultUnitCategory = UnitCategory.Countable, SortOrder = 7 },
            new Category { Id = 8, Name = "Enlatados", Description = "Conservas, atún, frijoles y productos enlatados", IconName = "can", DefaultUnitCategory = UnitCategory.Countable, SortOrder = 8 },
            new Category { Id = 9, Name = "Condimentos", Description = "Salsas, especias, aceites y aderezos", IconName = "seasoning", DefaultUnitCategory = UnitCategory.Liquid, SortOrder = 9 },
            new Category { Id = 10, Name = "Panadería", Description = "Pan, tortillas, bollería y repostería", IconName = "bread", DefaultUnitCategory = UnitCategory.Countable, SortOrder = 10 }
        );
    }
}
