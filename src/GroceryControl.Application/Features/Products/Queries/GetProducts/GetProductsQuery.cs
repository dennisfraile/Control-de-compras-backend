using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    int? CategoryId,
    int Page = 1,
    int PageSize = 20) : IRequest<List<ProductDto>>;
