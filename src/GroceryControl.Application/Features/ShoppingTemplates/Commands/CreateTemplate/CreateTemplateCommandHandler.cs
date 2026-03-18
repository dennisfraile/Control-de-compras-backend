using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.ShoppingTemplates.DTOs;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.ShoppingTemplates.Commands.CreateTemplate;

public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, ShoppingTemplateDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateTemplateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ShoppingTemplateDto> Handle(CreateTemplateCommand request, CancellationToken ct)
    {
        var template = new ShoppingTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = _currentUser.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            Items = request.Items.Select(i => new ShoppingTemplateItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitTypeId = i.UnitTypeId
            }).ToList()
        };

        _context.ShoppingTemplates.Add(template);
        await _context.SaveChangesAsync(ct);

        // Reload with includes
        var saved = await _context.ShoppingTemplates
            .AsNoTracking()
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .Include(t => t.Items).ThenInclude(i => i.UnitType)
            .FirstAsync(t => t.Id == template.Id, ct);

        return MapToDto(saved);
    }

    private static ShoppingTemplateDto MapToDto(ShoppingTemplate t) => new(
        t.Id, t.Name, t.CreatedAtUtc,
        t.Items.Select(i => new ShoppingTemplateItemDto(
            i.Id, i.ProductId, i.Product.Name, i.Quantity, i.UnitTypeId, i.UnitType.Abbreviation
        )).ToList()
    );
}
