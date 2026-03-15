using GroceryControl.Application.Features.Inventory.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Commands.ConsumeStock;

public record ConsumeStockCommand(
    Guid ProductId,
    decimal QuantityToConsume) : IRequest<InventoryEntryDto>;
