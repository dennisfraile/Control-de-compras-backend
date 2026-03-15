using GroceryControl.Domain.Interfaces;

namespace GroceryControl.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly GroceryControlDbContext _context;

    public UnitOfWork(GroceryControlDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
