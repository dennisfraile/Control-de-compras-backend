using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Queries.GetFavorites;

public record GetFavoritesQuery : IRequest<List<ProductDto>>;
