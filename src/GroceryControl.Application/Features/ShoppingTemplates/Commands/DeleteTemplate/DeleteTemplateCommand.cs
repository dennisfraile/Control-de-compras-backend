using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.ShoppingTemplates.Commands.DeleteTemplate;

public record DeleteTemplateCommand(Guid Id) : IRequest;

public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteTemplateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteTemplateCommand request, CancellationToken ct)
    {
        var template = await _context.ShoppingTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == _currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(ShoppingTemplate), request.Id);

        _context.ShoppingTemplates.Remove(template);
        await _context.SaveChangesAsync(ct);
    }
}
