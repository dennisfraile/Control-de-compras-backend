using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Commands.QuickPurchase;

public class QuickPurchaseCommandHandler : IRequestHandler<QuickPurchaseCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public QuickPurchaseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(QuickPurchaseCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var now = DateTime.UtcNow;

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoreId = request.StoreId,
            PurchaseDateUtc = now,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Notes = "Compra rapida",
            CreatedAtUtc = now,
            Items = new List<PurchaseItem>()
        };

        foreach (var item in request.Items)
        {
            purchase.Items.Add(new PurchaseItem
            {
                Id = Guid.NewGuid(),
                PurchaseId = purchase.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitTypeId = item.UnitTypeId,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice,
                AddToInventory = true
            });

            // Update inventory
            var inventory = await _context.InventoryEntries
                .FirstOrDefaultAsync(ie => ie.UserId == userId && ie.ProductId == item.ProductId, ct);

            if (inventory != null)
            {
                inventory.CurrentQuantity += item.Quantity;
                inventory.LastUpdatedUtc = now;
            }
            else
            {
                _context.InventoryEntries.Add(new InventoryEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = item.ProductId,
                    CurrentQuantity = item.Quantity,
                    UnitTypeId = item.UnitTypeId,
                    MinimumThreshold = 1,
                    LastUpdatedUtc = now
                });
            }

            // Add price suggestion
            _context.PriceSuggestions.Add(new PriceSuggestion
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                StoreId = request.StoreId,
                Price = item.UnitPrice,
                Quantity = item.Quantity,
                UnitTypeId = item.UnitTypeId,
                SubmittedByUserId = userId,
                ObservedDateUtc = now,
                CreatedAtUtc = now
            });
        }

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync(ct);

        return purchase.Id;
    }
}
