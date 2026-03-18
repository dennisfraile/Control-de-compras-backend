using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Recipes.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Recipes.Queries.GetRecipeSuggestions;

public class GetRecipeSuggestionsQueryHandler : IRequestHandler<GetRecipeSuggestionsQuery, List<RecipeSuggestionDto>>
{
    private readonly IRecipeSuggestionService _recipeSuggestionService;
    private readonly ICurrentUserService _currentUser;

    public GetRecipeSuggestionsQueryHandler(
        IRecipeSuggestionService recipeSuggestionService,
        ICurrentUserService currentUser)
    {
        _recipeSuggestionService = recipeSuggestionService;
        _currentUser = currentUser;
    }

    public async Task<List<RecipeSuggestionDto>> Handle(
        GetRecipeSuggestionsQuery request, CancellationToken cancellationToken)
    {
        return await _recipeSuggestionService.GetSuggestionsAsync(_currentUser.UserId, cancellationToken);
    }
}
