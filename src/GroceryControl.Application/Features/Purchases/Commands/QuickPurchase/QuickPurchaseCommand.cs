using MediatR;

namespace GroceryControl.Application.Features.Purchases.Commands.QuickPurchase;

public record QuickPurchaseItemDto(Guid ProductId, decimal Quantity, int UnitTypeId, decimal UnitPrice);

public record QuickPurchaseCommand(
    Guid StoreId,
    List<QuickPurchaseItemDto> Items
) : IRequest<Guid>;
