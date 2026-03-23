using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Brand,
    string? Barcode,
    int CategoryId,
    int DefaultUnitTypeId,
    decimal DefaultQuantity,
    string? ImageUrl,
    string? Notes,
    decimal PackageSize = 1,
    string? PackageLabel = null) : IRequest<ProductDto>;
