using GroceryControl.Application.Features.Products.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Products.Queries.GetProductByBarcode;

public record GetProductByBarcodeQuery(string Barcode) : IRequest<ProductDto?>;
