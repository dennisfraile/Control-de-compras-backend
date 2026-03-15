using GroceryControl.Application.Features.Categories.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
