using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Purchases.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Commands.CreatePurchase;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseDto>
{
    private readonly IRepository<Purchase> _purchaseRepository;
    private readonly IRepository<InventoryEntry> _inventoryRepository;
    private readonly IRepository<PriceSuggestion> _priceSuggestionRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandHandler(
        IRepository<Purchase> purchaseRepository,
        IRepository<InventoryEntry> inventoryRepository,
        IRepository<PriceSuggestion> priceSuggestionRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _inventoryRepository = inventoryRepository;
        _priceSuggestionRepository = priceSuggestionRepository;
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseDto> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.UserId,
            StoreId = request.StoreId,
            PurchaseDateUtc = request.PurchaseDateUtc,
            Notes = request.Notes,
            Tags = request.Tags,
            CreatedAtUtc = DateTime.UtcNow
        };

        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var totalPrice = item.Quantity * item.UnitPrice;
            totalAmount += totalPrice;

            var purchaseItem = new PurchaseItem
            {
                Id = Guid.NewGuid(),
                PurchaseId = purchase.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitTypeId = item.UnitTypeId,
                UnitPrice = item.UnitPrice,
                TotalPrice = totalPrice,
                AddToInventory = item.AddToInventory
            };

            purchase.Items.Add(purchaseItem);

            // Auto-create PriceSuggestion for community price data
            var priceSuggestion = new PriceSuggestion
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                StoreId = request.StoreId,
                Price = item.UnitPrice,
                Quantity = item.Quantity,
                UnitTypeId = item.UnitTypeId,
                SubmittedByUserId = _currentUser.UserId,
                ObservedDateUtc = request.PurchaseDateUtc,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _priceSuggestionRepository.AddAsync(priceSuggestion, cancellationToken);

            // Update inventory if flagged
            if (item.AddToInventory)
            {
                var inventoryEntry = await _context.InventoryEntries
                    .FirstOrDefaultAsync(
                        ie => ie.UserId == _currentUser.UserId && ie.ProductId == item.ProductId,
                        cancellationToken);

                if (inventoryEntry is not null)
                {
                    inventoryEntry.CurrentQuantity += item.Quantity;
                    inventoryEntry.LastUpdatedUtc = DateTime.UtcNow;
                    _inventoryRepository.Update(inventoryEntry);
                }
                else
                {
                    var newEntry = new InventoryEntry
                    {
                        Id = Guid.NewGuid(),
                        UserId = _currentUser.UserId,
                        ProductId = item.ProductId,
                        CurrentQuantity = item.Quantity,
                        UnitTypeId = item.UnitTypeId,
                        MinimumThreshold = 0,
                        LastUpdatedUtc = DateTime.UtcNow
                    };

                    await _inventoryRepository.AddAsync(newEntry, cancellationToken);
                }
            }
        }

        purchase.TotalAmount = totalAmount;

        await _purchaseRepository.AddAsync(purchase, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties for DTO
        var store = await _context.Stores
            .AsNoTracking()
            .FirstAsync(s => s.Id == purchase.StoreId, cancellationToken);

        var productIds = purchase.Items.Select(i => i.ProductId).ToList();
        var unitTypeIds = purchase.Items.Select(i => i.UnitTypeId).Distinct().ToList();

        var products = await _context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var unitTypes = await _context.UnitTypes
            .AsNoTracking()
            .Where(u => unitTypeIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var itemDtos = purchase.Items.Select(i => new PurchaseItemDto(
            i.Id,
            i.ProductId,
            products[i.ProductId].Name,
            products[i.ProductId].Brand,
            i.Quantity,
            i.UnitTypeId,
            unitTypes[i.UnitTypeId].Abbreviation,
            i.UnitPrice,
            i.TotalPrice,
            i.AddToInventory)).ToList();

        return new PurchaseDto(
            purchase.Id,
            purchase.UserId,
            purchase.StoreId,
            store.Name,
            purchase.PurchaseDateUtc,
            purchase.TotalAmount,
            purchase.Notes,
            purchase.CreatedAtUtc,
            itemDtos);
    }
}
