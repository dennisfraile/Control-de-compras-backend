using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchProductsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var term = request.SearchTerm.ToLower();

        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.DefaultUnitType)
            .Where(p => (p.IsGlobal || p.CreatedByUserId == _currentUser.UserId)
                && (p.Name.ToLower().Contains(term)
                    || (p.Brand != null && p.Brand.ToLower().Contains(term))))
            .OrderBy(p => p.Name)
            .Take(50)
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
