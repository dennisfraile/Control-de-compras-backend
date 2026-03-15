using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchaseSummary;

public record GetPurchaseSummaryQuery(int? Year, int? Month) : IRequest<List<PurchaseSummaryDto>>;
