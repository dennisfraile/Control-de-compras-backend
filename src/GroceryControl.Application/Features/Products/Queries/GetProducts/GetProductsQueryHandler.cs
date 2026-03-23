using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.DefaultUnitType)
            .Where(p => p.IsGlobal || p.CreatedByUserId == _currentUser.UserId)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Brand,
            p.Barcode,
            p.CategoryId,
            p.Category.Name,
            p.DefaultUnitTypeId,
            p.DefaultUnitType.Abbreviation,
            p.DefaultQuantity,
            p.ImageUrl,
            p.Notes,
            p.IsGlobal,
            p.CreatedByUserId,
            p.CreatedAtUtc)).ToList();
    }
}
