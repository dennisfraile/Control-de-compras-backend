using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.PriceSuggestions.Commands.SubmitPrice;

public class SubmitPriceCommandHandler : IRequestHandler<SubmitPriceCommand, PriceSuggestionDto>
{
    private readonly IRepository<PriceSuggestion> _priceSuggestionRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitPriceCommandHandler(
        IRepository<PriceSuggestion> priceSuggestionRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _priceSuggestionRepository = priceSuggestionRepository;
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<PriceSuggestionDto> Handle(SubmitPriceCommand request, CancellationToken cancellationToken)
    {
        var priceSuggestion = new PriceSuggestion
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            StoreId = request.StoreId,
            Price = request.Price,
            Quantity = request.Quantity,
            UnitTypeId = request.UnitTypeId,
            SubmittedByUserId = _currentUser.UserId,
            ObservedDateUtc = request.ObservedDateUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _priceSuggestionRepository.AddAsync(priceSuggestion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var product = await _context.Products
            .AsNoTracking()
            .FirstAsync(p => p.Id == priceSuggestion.ProductId, cancellationToken);

        var store = await _context.Stores
            .AsNoTracking()
            .FirstAsync(s => s.Id == priceSuggestion.StoreId, cancellationToken);

        var unitType = await _context.UnitTypes
            .AsNoTracking()
            .FirstAsync(u => u.Id == priceSuggestion.UnitTypeId, cancellationToken);

        return new PriceSuggestionDto(
            product.Name,
            store.Name,
            priceSuggestion.Price,
            priceSuggestion.Quantity,
            unitType.Abbreviation,
            priceSuggestion.ObservedDateUtc);
    }
}
