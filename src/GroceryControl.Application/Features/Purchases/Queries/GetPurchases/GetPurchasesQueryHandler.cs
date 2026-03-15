using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchases;

public class GetPurchasesQueryHandler : IRequestHandler<GetPurchasesQuery, List<PurchaseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchasesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PurchaseDto>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Purchases
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Include(p => p.Items)
                .ThenInclude(i => i.UnitType)
            .Where(p => p.UserId == _currentUser.UserId)
            .AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(p => p.PurchaseDateUtc >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.PurchaseDateUtc <= request.ToDate.Value);

        var purchases = await query
            .OrderByDescending(p => p.PurchaseDateUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return purchases.Select(p => new PurchaseDto(
            p.Id,
            p.UserId,
            p.StoreId,
            p.Store.Name,
            p.PurchaseDateUtc,
            p.TotalAmount,
            p.Notes,
            p.CreatedAtUtc,
            p.Items.Select(i => new PurchaseItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.Brand,
                i.Quantity,
                i.UnitTypeId,
                i.UnitType.Abbreviation,
                i.UnitPrice,
                i.TotalPrice,
                i.AddToInventory)).ToList())).ToList();
    }
}
