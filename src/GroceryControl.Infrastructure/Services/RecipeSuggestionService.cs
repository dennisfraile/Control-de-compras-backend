using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Recipes.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Infrastructure.Services;

public class RecipeSuggestionService : IRecipeSuggestionService
{
    private readonly IApplicationDbContext _context;

    public RecipeSuggestionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecipeSuggestionDto>> GetSuggestionsAsync(Guid userId, CancellationToken ct)
    {
        var inventoryNames = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Where(ie => ie.UserId == userId && ie.CurrentQuantity > 0)
            .Select(ie => ie.Product.Name.ToLower())
            .ToListAsync(ct);

        var suggestions = new List<RecipeSuggestionDto>();

        foreach (var recipe in Recipes)
        {
            var ingredients = new List<RecipeIngredientDto>();
            var missing = new List<string>();

            foreach (var (name, qty, unit) in recipe.Ingredients)
            {
                bool inStock = inventoryNames.Any(inv =>
                    inv.Contains(name.ToLower()) || name.ToLower().Contains(inv));

                ingredients.Add(new RecipeIngredientDto(name, qty, unit, inStock));

                if (!inStock)
                    missing.Add(name);
            }

            int total = recipe.Ingredients.Count;
            int matched = total - missing.Count;
            int matchPercentage = (int)((double)matched / total * 100);

            if (matchPercentage >= 50)
            {
                suggestions.Add(new RecipeSuggestionDto(
                    recipe.Name,
                    recipe.Description,
                    ingredients,
                    matchPercentage,
                    missing));
            }
        }

        return suggestions
            .OrderByDescending(s => s.MatchPercentage)
            .ToList();
    }

    private record RecipeDefinition(
        string Name,
        string Description,
        List<(string Name, decimal Quantity, string Unit)> Ingredients);

    private static readonly List<RecipeDefinition> Recipes =
    [
        new("Arroz con pollo",
            "Clasico arroz con pollo, un platillo completo y reconfortante.",
            [("Arroz", 2, "tazas"), ("Pollo", 500, "g"), ("Cebolla", 1, "pza"), ("Ajo", 3, "dientes"), ("Tomate", 2, "pza"), ("Aceite", 2, "cucharadas")]),

        new("Frijoles refritos",
            "Frijoles machacados y fritos con cebolla y manteca, perfectos como acompanamiento.",
            [("Frijol", 500, "g"), ("Cebolla", 1, "pza"), ("Ajo", 2, "dientes"), ("Manteca", 3, "cucharadas"), ("Sal", 1, "cucharadita")]),

        new("Huevos rancheros",
            "Huevos fritos sobre tortilla banados en salsa roja picante.",
            [("Huevo", 4, "pza"), ("Tortilla", 4, "pza"), ("Tomate", 3, "pza"), ("Chile", 2, "pza"), ("Cebolla", 1, "pza"), ("Aceite", 2, "cucharadas")]),

        new("Pasta con salsa de tomate",
            "Pasta sencilla con salsa casera de tomate, ajo y albahaca.",
            [("Pasta", 250, "g"), ("Tomate", 4, "pza"), ("Ajo", 3, "dientes"), ("Cebolla", 1, "pza"), ("Aceite de oliva", 2, "cucharadas"), ("Sal", 1, "cucharadita")]),

        new("Ensalada cesar",
            "Ensalada fresca con lechuga romana, croutones y aderezo cesar.",
            [("Lechuga", 1, "pza"), ("Pan", 2, "rebanadas"), ("Queso parmesano", 50, "g"), ("Limon", 1, "pza"), ("Aceite de oliva", 3, "cucharadas"), ("Ajo", 1, "diente")]),

        new("Quesadillas",
            "Tortillas rellenas de queso derretido, sencillas y rapidas.",
            [("Tortilla", 6, "pza"), ("Queso", 300, "g"), ("Aceite", 1, "cucharada")]),

        new("Chilaquiles verdes",
            "Totopos banados en salsa verde con crema y queso fresco.",
            [("Tortilla", 8, "pza"), ("Tomate verde", 500, "g"), ("Chile serrano", 3, "pza"), ("Cebolla", 1, "pza"), ("Crema", 100, "ml"), ("Queso fresco", 100, "g"), ("Aceite", 3, "cucharadas")]),

        new("Tacos de frijol",
            "Tacos sencillos de frijoles refritos con queso y salsa.",
            [("Tortilla", 6, "pza"), ("Frijol", 300, "g"), ("Queso", 150, "g"), ("Cebolla", 1, "pza"), ("Sal", 1, "cucharadita")]),

        new("Sopa de fideos",
            "Sopa caldosa de fideos tostados en caldo de tomate.",
            [("Fideo", 200, "g"), ("Tomate", 3, "pza"), ("Ajo", 2, "dientes"), ("Cebolla", 1, "pza"), ("Aceite", 2, "cucharadas"), ("Sal", 1, "cucharadita")]),

        new("Arroz rojo",
            "Arroz frito y cocido en salsa de tomate, guarnicion mexicana clasica.",
            [("Arroz", 2, "tazas"), ("Tomate", 2, "pza"), ("Ajo", 1, "diente"), ("Cebolla", 1, "pza"), ("Aceite", 2, "cucharadas"), ("Sal", 1, "cucharadita")]),

        new("Enfrijoladas",
            "Tortillas banadas en salsa de frijol con crema y queso.",
            [("Tortilla", 8, "pza"), ("Frijol", 400, "g"), ("Crema", 100, "ml"), ("Queso fresco", 100, "g"), ("Cebolla", 1, "pza"), ("Aceite", 1, "cucharada")]),

        new("Sincronizadas",
            "Tortillas de harina rellenas de jamon y queso, doradas en comal.",
            [("Tortilla de harina", 4, "pza"), ("Jamon", 200, "g"), ("Queso", 200, "g"), ("Aceite", 1, "cucharada")]),

        new("Molletes",
            "Pan bolillo abierto con frijoles y queso gratinado.",
            [("Pan bolillo", 4, "pza"), ("Frijol", 200, "g"), ("Queso", 150, "g"), ("Salsa", 100, "ml")]),

        new("Caldo de pollo",
            "Caldo reconfortante con pollo, verduras y arroz.",
            [("Pollo", 500, "g"), ("Zanahoria", 2, "pza"), ("Papa", 2, "pza"), ("Calabaza", 1, "pza"), ("Cebolla", 1, "pza"), ("Ajo", 2, "dientes"), ("Sal", 1, "cucharadita")]),

        new("Tostadas de tinga",
            "Tostadas crujientes con pollo deshebrado en salsa de chipotle.",
            [("Pollo", 400, "g"), ("Tomate", 3, "pza"), ("Chipotle", 2, "pza"), ("Cebolla", 1, "pza"), ("Tostada", 6, "pza"), ("Crema", 100, "ml"), ("Lechuga", 1, "pza")])
    ];
}
