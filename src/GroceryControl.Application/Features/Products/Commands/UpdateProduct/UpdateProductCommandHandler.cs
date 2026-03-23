using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Products.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
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

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (product.CreatedByUserId != _currentUser.UserId && !product.IsGlobal)
            throw new ForbiddenException("You can only update your own products.");

        product.Name = request.Name;
        product.Brand = request.Brand;
        product.Barcode = request.Barcode;
        product.CategoryId = request.CategoryId;
        product.DefaultUnitTypeId = request.DefaultUnitTypeId;
        product.DefaultQuantity = request.DefaultQuantity;
        product.ImageUrl = request.ImageUrl;
        product.Notes = request.Notes;
        product.PackageSize = request.PackageSize;
        product.PackageLabel = request.PackageLabel;

        _productRepository.Update(product);
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
            product.Notes,
            product.PackageSize,
            product.PackageLabel,
            product.IsGlobal,
            product.CreatedByUserId,
            product.CreatedAtUtc);
    }
}
