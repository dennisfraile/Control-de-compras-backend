using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Stores.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, StoreDto>
{
    private readonly IRepository<Store> _storeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStoreCommandHandler(
        IRepository<Store> storeRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<StoreDto> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Branch = request.Branch,
            Address = request.Address,
            City = request.City,
            IsGlobal = false,
            CreatedByUserId = _currentUser.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _storeRepository.AddAsync(store, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StoreDto(
            store.Id,
            store.Name,
            store.Branch,
            store.Address,
            store.City,
            store.IsGlobal,
            store.CreatedByUserId,
            store.CreatedAtUtc);
    }
}
