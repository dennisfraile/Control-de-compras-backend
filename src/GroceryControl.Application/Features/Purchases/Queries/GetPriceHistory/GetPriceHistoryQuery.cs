using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPriceHistory;

public record GetPriceHistoryQuery(Guid ProductId) : IRequest<List<PriceHistoryDto>>;
