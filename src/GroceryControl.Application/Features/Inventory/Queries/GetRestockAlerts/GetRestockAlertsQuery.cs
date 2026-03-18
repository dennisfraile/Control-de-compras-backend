using GroceryControl.Application.Common.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Queries.GetRestockAlerts;

public record GetRestockAlertsQuery : IRequest<List<RestockAlert>>;
