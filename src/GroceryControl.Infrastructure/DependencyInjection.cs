using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Domain.Interfaces;
using GroceryControl.Infrastructure.Authentication;
using GroceryControl.Infrastructure.Persistence;
using GroceryControl.Infrastructure.Persistence.Repositories;
using GroceryControl.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<GroceryControlDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories & UnitOfWork
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Authentication
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        // Services
        services.AddScoped<IShoppingListGenerator, ShoppingListGenerator>();

        return services;
    }
}
