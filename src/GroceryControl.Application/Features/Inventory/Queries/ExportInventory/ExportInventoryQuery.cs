using MediatR;

namespace GroceryControl.Application.Features.Inventory.Queries.ExportInventory;

public record ExportInventoryQuery : IRequest<byte[]>;
