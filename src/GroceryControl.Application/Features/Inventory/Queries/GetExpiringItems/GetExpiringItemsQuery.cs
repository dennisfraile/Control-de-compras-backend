using GroceryControl.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Inventory.Queries.GetExpiringItems;

public record ExpiringItemDto(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    decimal CurrentQuantity,
    string UnitAbbreviation,
    DateTime ExpirationDate,
    int DaysUntilExpiry
);

public record GetExpiringItemsQuery(int DaysAhead = 7) : IRequest<List<ExpiringItemDto>>;

public class GetExpiringItemsQueryHandler : IRequestHandler<GetExpiringItemsQuery, List<ExpiringItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetExpiringItemsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ExpiringItemDto>> Handle(GetExpiringItemsQuery request, CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddDays(request.DaysAhead);

        var items = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product).ThenInclude(p => p.Category)
            .Include(ie => ie.UnitType)
            .Where(ie => ie.UserId == _currentUser.UserId
                && ie.ExpirationDateUtc != null
                && ie.ExpirationDateUtc <= cutoff
                && ie.CurrentQuantity > 0)
            .OrderBy(ie => ie.ExpirationDateUtc)
            .ToListAsync(ct);

        return items.Select(ie => new ExpiringItemDto(
            ie.ProductId,
            ie.Product.Name,
            ie.Product.Category.Name,
            ie.CurrentQuantity,
            ie.UnitType.Abbreviation,
            ie.ExpirationDateUtc!.Value,
            (int)(ie.ExpirationDateUtc.Value - DateTime.UtcNow).TotalDays
        )).ToList();
    }
}
