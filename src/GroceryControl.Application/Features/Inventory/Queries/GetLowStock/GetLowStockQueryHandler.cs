using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Inventory.Queries.GetLowStock;

public class GetLowStockQueryHandler : IRequestHandler<GetLowStockQuery, List<InventoryEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetLowStockQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<InventoryEntryDto>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
        return await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
            .Include(ie => ie.UnitType)
            .Where(ie => ie.UserId == _currentUser.UserId
                && ie.CurrentQuantity <= ie.MinimumThreshold)
            .OrderBy(ie => ie.CurrentQuantity)
            .Select(ie => new InventoryEntryDto(
                ie.Id,
                ie.UserId,
                ie.ProductId,
                ie.Product.Name,
                ie.Product.Brand,
                ie.CurrentQuantity,
                ie.UnitTypeId,
                ie.UnitType.Abbreviation,
                ie.MinimumThreshold,
                ie.LastUpdatedUtc))
            .ToListAsync(cancellationToken);
    }
}
