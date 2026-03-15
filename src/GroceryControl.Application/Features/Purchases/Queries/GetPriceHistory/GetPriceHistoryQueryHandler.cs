using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPriceHistory;

public class GetPriceHistoryQueryHandler : IRequestHandler<GetPriceHistoryQuery, List<PriceHistoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPriceHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PriceHistoryDto>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _context.PurchaseItems
            .AsNoTracking()
            .Include(pi => pi.Purchase)
                .ThenInclude(p => p.Store)
            .Include(pi => pi.UnitType)
            .Where(pi => pi.ProductId == request.ProductId
                && pi.Purchase.UserId == _currentUser.UserId)
            .OrderByDescending(pi => pi.Purchase.PurchaseDateUtc)
            .Select(pi => new PriceHistoryDto(
                pi.Purchase.PurchaseDateUtc,
                pi.Purchase.Store.Name,
                pi.UnitPrice,
                pi.Quantity,
                pi.UnitType.Abbreviation))
            .ToListAsync(cancellationToken);
    }
}
