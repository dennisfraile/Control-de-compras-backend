using GroceryControl.Application.Features.Inventory.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Queries.GetInventory;

public record GetInventoryQuery : IRequest<List<InventoryEntryDto>>;
