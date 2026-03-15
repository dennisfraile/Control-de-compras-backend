using GroceryControl.Application.Features.Inventory.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Queries.GetLowStock;

public record GetLowStockQuery : IRequest<List<InventoryEntryDto>>;
