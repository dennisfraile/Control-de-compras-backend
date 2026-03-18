using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Commands.ToggleFavorite;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ToggleFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var existing = await _context.UserFavoriteProducts
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == request.ProductId, ct);

        if (existing != null)
        {
            _context.UserFavoriteProducts.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return false; // unfavorited
        }

        _context.UserFavoriteProducts.Add(new UserFavoriteProduct
        {
            UserId = userId,
            ProductId = request.ProductId,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(ct);
        return true; // favorited
    }
}
