using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.PriceSuggestions.Queries.GetLowestPrices;

public class GetLowestPricesQueryHandler : IRequestHandler<GetLowestPricesQuery, List<PriceSuggestionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowestPricesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PriceSuggestionDto>> Handle(GetLowestPricesQuery request, CancellationToken cancellationToken)
    {
        return await _context.PriceSuggestions
            .AsNoTracking()
            .Include(ps => ps.Product)
            .Include(ps => ps.Store)
            .Include(ps => ps.UnitType)
            .Where(ps => ps.ProductId == request.ProductId)
            .OrderBy(ps => ps.Price)
            .Take(10)
            .Select(ps => new PriceSuggestionDto(
                ps.Product.Name,
                ps.Store.Name,
                ps.Price,
                ps.Quantity,
                ps.UnitType.Abbreviation,
                ps.ObservedDateUtc))
            .ToListAsync(cancellationToken);
    }
}
