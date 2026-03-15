using MediatR;

namespace GroceryControl.Application.Features.Purchases.Commands.DeletePurchase;

public record DeletePurchaseCommand(Guid Id) : IRequest<Unit>;
