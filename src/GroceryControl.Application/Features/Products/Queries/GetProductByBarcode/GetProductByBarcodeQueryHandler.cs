using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Queries.GetProductByBarcode;

public class GetProductByBarcodeQueryHandler : IRequestHandler<GetProductByBarcodeQuery, ProductDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductByBarcodeQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductDto?> Handle(GetProductByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.DefaultUnitType)
            .Where(p => p.Barcode == request.Barcode
                && (p.IsGlobal || p.CreatedByUserId == _currentUser.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return null;

        return new ProductDto(
            product.Id,
            product.Name,
            product.Brand,
            product.Barcode,
            product.CategoryId,
            product.Category.Name,
            product.DefaultUnitTypeId,
            product.DefaultUnitType.Abbreviation,
            product.DefaultQuantity,
            product.ImageUrl,
            product.Notes,
            product.IsGlobal,
            product.CreatedByUserId,
            product.CreatedAtUtc);
    }
}
