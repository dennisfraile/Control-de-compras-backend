using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Stores.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Stores.Queries.GetStores;

public class GetStoresQueryHandler : IRequestHandler<GetStoresQuery, List<StoreDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetStoresQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<StoreDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.IsGlobal || s.CreatedByUserId == _currentUser.UserId)
            .OrderBy(s => s.Name)
            .Select(s => new StoreDto(
                s.Id,
                s.Name,
                s.Branch,
                s.Address,
                s.City,
                s.IsGlobal,
                s.CreatedByUserId,
                s.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
