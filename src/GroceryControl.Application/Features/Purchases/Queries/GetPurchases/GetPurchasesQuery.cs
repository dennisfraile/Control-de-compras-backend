using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchases;

public record GetPurchasesQuery(
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 20) : IRequest<List<PurchaseDto>>;
