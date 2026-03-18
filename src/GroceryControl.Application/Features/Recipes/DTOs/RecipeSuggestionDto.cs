namespace GroceryControl.Application.Features.Recipes.DTOs;

public record RecipeIngredientDto(
    string ProductName,
    decimal Quantity,
    string Unit,
    bool InStock);

public record RecipeSuggestionDto(
    string Name,
    string Description,
    List<RecipeIngredientDto> Ingredients,
    int MatchPercentage,
    List<string> MissingIngredients);
