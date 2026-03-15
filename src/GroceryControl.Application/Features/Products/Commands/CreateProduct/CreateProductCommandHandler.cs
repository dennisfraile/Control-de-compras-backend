using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Brand = request.Brand,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            DefaultUnitTypeId = request.DefaultUnitTypeId,
            DefaultQuantity = request.DefaultQuantity,
            ImageUrl = request.ImageUrl,
            IsGlobal = false,
            CreatedByUserId = _currentUser.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var category = await _context.Categories
            .AsNoTracking()
            .FirstAsync(c => c.Id == product.CategoryId, cancellationToken);

        var unitType = await _context.UnitTypes
            .AsNoTracking()
            .FirstAsync(u => u.Id == product.DefaultUnitTypeId, cancellationToken);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Brand,
            product.Barcode,
            product.CategoryId,
            category.Name,
            product.DefaultUnitTypeId,
            unitType.Abbreviation,
            product.DefaultQuantity,
            product.ImageUrl,
            product.IsGlobal,
            product.CreatedByUserId,
            product.CreatedAtUtc);
    }
}
