using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Commands.CreatePurchase;

public record CreatePurchaseCommand(
    Guid StoreId,
    DateTime PurchaseDateUtc,
    string? Notes,
    string? Tags,
    List<CreatePurchaseItemCommand> Items) : IRequest<PurchaseDto>;

public record CreatePurchaseItemCommand(
    Guid ProductId,
    decimal Quantity,
    int UnitTypeId,
    decimal UnitPrice,
    bool AddToInventory);
