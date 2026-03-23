using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Queries.GetProductPriceDetail;

public class GetProductPriceDetailQueryHandler : IRequestHandler<GetProductPriceDetailQuery, ProductPriceDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductPriceDetailQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductPriceDetailDto> Handle(GetProductPriceDetailQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new Application.Common.Exceptions.NotFoundException("Producto", request.ProductId);

        var purchases = await _context.PurchaseItems
            .AsNoTracking()
            .Include(pi => pi.Purchase).ThenInclude(p => p.Store)
            .Where(pi => pi.ProductId == request.ProductId && pi.Purchase.UserId == _currentUser.UserId)
            .OrderByDescending(pi => pi.Purchase.PurchaseDateUtc)
            .ToListAsync(cancellationToken);

        var history = purchases.Select(pi => new PriceHistoryEntry(
            pi.Purchase.PurchaseDateUtc,
            pi.Purchase.Store.Name,
            pi.UnitPrice,
            pi.Quantity
        )).ToList();

        if (history.Count == 0)
        {
            return new ProductPriceDetailDto(
                product.Name, product.Brand, product.Category.Name,
                0, 0, "-", 0, "-", "stable", history);
        }

        var avgPrice = history.Average(h => h.UnitPrice);
        var cheapest = history.MinBy(h => h.UnitPrice)!;
        var expensive = history.MaxBy(h => h.UnitPrice)!;

        var last3 = history.Take(3).Select(h => h.UnitPrice).ToList();
        var trend = "stable";
        if (last3.Count >= 2)
        {
            if (last3[0] > last3[^1]) trend = "up";
            else if (last3[0] < last3[^1]) trend = "down";
        }

        return new ProductPriceDetailDto(
            product.Name, product.Brand, product.Category.Name,
            Math.Round(avgPrice, 2), cheapest.UnitPrice, cheapest.StoreName,
            expensive.UnitPrice, expensive.StoreName, trend, history);
    }
}
