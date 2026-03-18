using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.ShoppingTemplates.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.ShoppingTemplates.Queries.GetTemplates;

public record GetTemplatesQuery : IRequest<List<ShoppingTemplateDto>>;

public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, List<ShoppingTemplateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTemplatesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ShoppingTemplateDto>> Handle(GetTemplatesQuery request, CancellationToken ct)
    {
        var templates = await _context.ShoppingTemplates
            .AsNoTracking()
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .Include(t => t.Items).ThenInclude(i => i.UnitType)
            .Where(t => t.UserId == _currentUser.UserId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(ct);

        return templates.Select(t => new ShoppingTemplateDto(
            t.Id, t.Name, t.CreatedAtUtc,
            t.Items.Select(i => new ShoppingTemplateItemDto(
                i.Id, i.ProductId, i.Product.Name, i.Quantity, i.UnitTypeId, i.UnitType.Abbreviation
            )).ToList()
        )).ToList();
    }
}
