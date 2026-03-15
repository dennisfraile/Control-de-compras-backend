using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Inventory.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Inventory.Commands.ConsumeStock;

public class ConsumeStockCommandHandler : IRequestHandler<ConsumeStockCommand, InventoryEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<InventoryEntry> _inventoryRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeStockCommandHandler(
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

    public async Task<InventoryEntryDto> Handle(ConsumeStockCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.InventoryEntries
            .FirstOrDefaultAsync(
                ie => ie.UserId == _currentUser.UserId && ie.ProductId == request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(InventoryEntry), request.ProductId);

        entry.CurrentQuantity = Math.Max(0, entry.CurrentQuantity - request.QuantityToConsume);
        entry.LastUpdatedUtc = DateTime.UtcNow;

        _inventoryRepository.Update(entry);
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
            entry.LastUpdatedUtc);
    }
}
