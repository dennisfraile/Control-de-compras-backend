using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Inventory.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Inventory.Commands.UpdateStock;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, InventoryEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<InventoryEntry> _inventoryRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockCommandHandler(
        IApplicationDbContext context,
        IRepository<InventoryEntry> inventoryRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryEntryDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.InventoryEntries
            .FirstOrDefaultAsync(
                ie => ie.UserId == _currentUser.UserId && ie.ProductId == request.ProductId,
                cancellationToken);

        if (entry is not null)
        {
            entry.CurrentQuantity = request.NewQuantity;
            entry.UnitTypeId = request.UnitTypeId;
            entry.MinimumThreshold = request.MinimumThreshold;
            entry.ExpirationDateUtc = request.ExpirationDateUtc;
            entry.LastUpdatedUtc = DateTime.UtcNow;
            _inventoryRepository.Update(entry);
        }
        else
        {
            entry = new InventoryEntry
            {
                Id = Guid.NewGuid(),
                UserId = _currentUser.UserId,
                ProductId = request.ProductId,
                CurrentQuantity = request.NewQuantity,
                UnitTypeId = request.UnitTypeId,
                MinimumThreshold = request.MinimumThreshold,
                ExpirationDateUtc = request.ExpirationDateUtc,
                LastUpdatedUtc = DateTime.UtcNow
            };
            await _inventoryRepository.AddAsync(entry, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var product = await _context.Products
            .AsNoTracking()
            .FirstAsync(p => p.Id == entry.ProductId, cancellationToken);

        var unitType = await _context.UnitTypes
            .AsNoTracking()
            .FirstAsync(u => u.Id == entry.UnitTypeId, cancellationToken);

        return new InventoryEntryDto(
            entry.Id,
            entry.UserId,
            entry.ProductId,
            product.Name,
            product.Brand,
            entry.CurrentQuantity,
            entry.UnitTypeId,
            unitType.Abbreviation,
            entry.MinimumThreshold,
            entry.LastUpdatedUtc,
            entry.ExpirationDateUtc);
    }
}
