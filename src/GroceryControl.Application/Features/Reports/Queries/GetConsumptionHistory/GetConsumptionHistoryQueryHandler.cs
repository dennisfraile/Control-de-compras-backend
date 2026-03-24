using GroceryControl.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Reports.Queries.GetConsumptionHistory;

public class GetConsumptionHistoryQueryHandler : IRequestHandler<GetConsumptionHistoryQuery, ConsumptionHistoryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetConsumptionHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ConsumptionHistoryDto> Handle(GetConsumptionHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);

        var purchases = await _context.PurchaseItems
            .Include(pi => pi.Purchase)
            .Where(pi => pi.ProductId == request.ProductId && pi.Purchase!.UserId == userId)
            .OrderBy(pi => pi.Purchase!.PurchaseDateUtc)
            .Select(pi => new
            {
                Date = pi.Purchase!.PurchaseDateUtc,
                pi.Quantity,
                pi.TotalPrice
            })
            .ToListAsync(cancellationToken);

        // Group by week
        var weekly = purchases
            .GroupBy(p =>
            {
                var date = p.Date;
                var startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                return startOfWeek.ToString("yyyy-MM-dd");
            })
            .Select(g => new ConsumptionDataPoint
            {
                Period = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                TotalSpent = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Period)
            .Take(12)
            .Reverse()
            .ToList();

        // Group by month
        var monthly = purchases
            .GroupBy(p => p.Date.ToString("yyyy-MM"))
            .Select(g => new ConsumptionDataPoint
            {
                Period = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                TotalSpent = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Period)
            .Take(6)
            .Reverse()
            .ToList();

        return new ConsumptionHistoryDto
        {
            ProductId = request.ProductId,
            ProductName = product?.Name ?? "Desconocido",
            WeeklyConsumption = weekly,
            MonthlyConsumption = monthly,
            AverageWeeklyConsumption = weekly.Count > 0 ? weekly.Average(w => w.Quantity) : 0,
            AverageMonthlyConsumption = monthly.Count > 0 ? monthly.Average(m => m.Quantity) : 0
        };
    }
}
