using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.PriceSuggestions.Queries.GetBestDeals;

public class GetBestDealsQueryHandler : IRequestHandler<GetBestDealsQuery, List<PriceSuggestionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBestDealsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PriceSuggestionDto>> Handle(GetBestDealsQuery request, CancellationToken cancellationToken)
    {
        // Get the most recent lowest price per product-store combination
        var query = _context.PriceSuggestions
            .AsNoTracking()
            .Include(ps => ps.Product)
            .Include(ps => ps.Store)
            .Include(ps => ps.UnitType)
            .Where(ps => ps.ObservedDateUtc >= DateTime.UtcNow.AddDays(-30))
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(ps => ps.Product.CategoryId == request.CategoryId.Value);
        }

        var bestDeals = await query
            .GroupBy(ps => new { ps.ProductId, ps.StoreId })
            .Select(g => g.OrderBy(ps => ps.Price).First())
            .OrderBy(ps => ps.Price)
            .Take(request.Limit)
            .Select(ps => new PriceSuggestionDto(
                ps.Product.Name,
                ps.Store.Name,
                ps.Price,
                ps.Quantity,
                ps.UnitType.Abbreviation,
                ps.ObservedDateUtc))
            .ToListAsync(cancellationToken);

        return bestDeals;
    }
}
