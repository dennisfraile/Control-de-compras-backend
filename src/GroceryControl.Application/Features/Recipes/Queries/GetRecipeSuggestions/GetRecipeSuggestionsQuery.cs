using GroceryControl.Application.Features.Recipes.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Recipes.Queries.GetRecipeSuggestions;

public record GetRecipeSuggestionsQuery : IRequest<List<RecipeSuggestionDto>>;
