using System.Text;
using GroceryControl.Application.Common.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.ShoppingList.Queries.GetShareText;

public class GetShoppingListShareTextQueryHandler : IRequestHandler<GetShoppingListShareTextQuery, string>
{
    private readonly IShoppingListGenerator _shoppingListGenerator;
    private readonly ICurrentUserService _currentUser;

    public GetShoppingListShareTextQueryHandler(
        IShoppingListGenerator shoppingListGenerator,
        ICurrentUserService currentUser)
    {
        _shoppingListGenerator = shoppingListGenerator;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(GetShoppingListShareTextQuery request, CancellationToken cancellationToken)
    {
        var items = await _shoppingListGenerator.GenerateAsync(_currentUser.UserId, cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("\U0001F6D2 Mi lista de compras");
        sb.AppendLine();

        foreach (var item in items)
        {
            sb.AppendLine($"\u25A1 {item.ProductName} x{item.SuggestedQuantity} {item.UnitAbbreviation}");
        }

        var total = items
            .Where(i => i.LowestKnownPrice.HasValue)
            .Sum(i => i.LowestKnownPrice!.Value * i.SuggestedQuantity);

        if (total > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"Total estimado: ${total:N0}");
        }

        sb.AppendLine("Generado con Mis compras");

        return sb.ToString();
    }
}
