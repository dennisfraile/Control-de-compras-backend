using AutoMapper;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Queries.GetFavorites;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetFavoritesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetFavoritesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var favoriteProductIds = await _context.UserFavoriteProducts
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync(ct);

        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.DefaultUnitType)
            .Where(p => favoriteProductIds.Contains(p.Id))
            .ToListAsync(ct);

        return _mapper.Map<List<ProductDto>>(products);
    }
}
