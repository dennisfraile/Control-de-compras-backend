using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Brand,
    string? Barcode,
    int CategoryId,
    int DefaultUnitTypeId,
    decimal DefaultQuantity,
    string? ImageUrl,
    string? Notes) : IRequest<ProductDto>;
