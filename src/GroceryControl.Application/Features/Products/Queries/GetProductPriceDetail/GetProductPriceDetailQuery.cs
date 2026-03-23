using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Queries.GetProductPriceDetail;

public record GetProductPriceDetailQuery(Guid ProductId) : IRequest<ProductPriceDetailDto>;
