using GroceryControl.Application.Features.Recipes.DTOs;

namespace GroceryControl.Application.Common.Interfaces;

public interface IRecipeSuggestionService
{
    Task<List<RecipeSuggestionDto>> GetSuggestionsAsync(Guid userId, CancellationToken ct);
}
