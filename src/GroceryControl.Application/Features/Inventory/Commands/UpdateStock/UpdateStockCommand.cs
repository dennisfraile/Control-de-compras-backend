using GroceryControl.Application.Features.Inventory.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Commands.UpdateStock;

public record UpdateStockCommand(
    Guid ProductId,
    decimal NewQuantity,
    int UnitTypeId,
    decimal MinimumThreshold) : IRequest<InventoryEntryDto>;
