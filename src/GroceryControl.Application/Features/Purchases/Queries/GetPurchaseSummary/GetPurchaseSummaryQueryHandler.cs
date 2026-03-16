using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchaseSummary;

public class GetPurchaseSummaryQueryHandler : IRequestHandler<GetPurchaseSummaryQuery, List<PurchaseSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchaseSummaryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PurchaseSummaryDto>> Handle(GetPurchaseSummaryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == _currentUser.UserId);

        if (request.Year.HasValue)
            query = query.Where(p => p.PurchaseDateUtc.Year == request.Year.Value);

        if (request.Month.HasValue)
            query = query.Where(p => p.PurchaseDateUtc.Month == request.Month.Value);

        var purchases = await query
            .Select(p => new { p.PurchaseDateUtc, p.TotalAmount })
            .ToListAsync(cancellationToken);

        var summaries = purchases
            .GroupBy(p => new { p.PurchaseDateUtc.Year, p.PurchaseDateUtc.Month })
            .Select(g => new PurchaseSummaryDto(
                g.Key.Year,
                g.Key.Month,
                g.Sum(p => p.TotalAmount),
                g.Count(),
                0))
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ToList();

        return summaries;
    }
}
