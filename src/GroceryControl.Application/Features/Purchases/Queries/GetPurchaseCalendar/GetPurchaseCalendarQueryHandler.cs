using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchaseCalendar;

public class GetPurchaseCalendarQueryHandler : IRequestHandler<GetPurchaseCalendarQuery, List<PurchaseCalendarDayDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchaseCalendarQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PurchaseCalendarDayDto>> Handle(GetPurchaseCalendarQuery request, CancellationToken cancellationToken)
    {
        var purchases = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Store)
            .Where(p => p.UserId == _currentUser.UserId
                && p.PurchaseDateUtc.Year == request.Year
                && p.PurchaseDateUtc.Month == request.Month)
            .ToListAsync(cancellationToken);

        var result = purchases
            .GroupBy(p => p.PurchaseDateUtc.Date)
            .Select(g => new PurchaseCalendarDayDto(
                g.Key,
                g.Count(),
                g.Sum(p => p.TotalAmount),
                g.Select(p => p.Store.Name).Distinct().ToList()
            ))
            .OrderBy(d => d.Date)
            .ToList();

        return result;
    }
}
