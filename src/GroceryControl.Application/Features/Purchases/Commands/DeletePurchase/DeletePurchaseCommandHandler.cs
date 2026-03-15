using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Commands.DeletePurchase;

public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand, Unit>
{
    private readonly IRepository<Purchase> _purchaseRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePurchaseCommandHandler(
        IRepository<Purchase> purchaseRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Purchase), request.Id);

        if (purchase.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only delete your own purchases.");

        _purchaseRepository.Delete(purchase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
