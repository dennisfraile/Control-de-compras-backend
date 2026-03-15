using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.ShoppingList.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.ShoppingList.Queries.GenerateShoppingList;

public class GenerateShoppingListQueryHandler : IRequestHandler<GenerateShoppingListQuery, List<ShoppingListSuggestionDto>>
{
    private readonly IShoppingListGenerator _shoppingListGenerator;
    private readonly ICurrentUserService _currentUser;

    public GenerateShoppingListQueryHandler(
        IShoppingListGenerator shoppingListGenerator,
        ICurrentUserService currentUser)
    {
        _shoppingListGenerator = shoppingListGenerator;
        _currentUser = currentUser;
    }

    public async Task<List<ShoppingListSuggestionDto>> Handle(
        GenerateShoppingListQuery request,
        CancellationToken cancellationToken)
    {
        return await _shoppingListGenerator.GenerateAsync(_currentUser.UserId, cancellationToken);
    }
}
