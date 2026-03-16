using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.ExportPurchases;

public record ExportPurchasesQuery(
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<byte[]>;
