using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(string SearchTerm) : IRequest<List<ProductDto>>;
