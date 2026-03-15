using AutoMapper;
using GroceryControl.Application.Features.Auth.DTOs;
using GroceryControl.Application.Features.Categories.DTOs;
using GroceryControl.Application.Features.Inventory.DTOs;
using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using GroceryControl.Application.Features.Products.DTOs;
using GroceryControl.Application.Features.Purchases.DTOs;
using GroceryControl.Application.Features.Stores.DTOs;
using GroceryControl.Application.Features.Users.DTOs;
using GroceryControl.Domain.Entities;

namespace GroceryControl.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserProfile, UserProfileDto>();

        CreateMap<Category, CategoryDto>();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.DefaultUnitType.Abbreviation));

        CreateMap<Store, StoreDto>();

        CreateMap<Purchase, PurchaseDto>()
            .ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store.Name));

        CreateMap<PurchaseItem, PurchaseItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductBrand, opt => opt.MapFrom(s => s.Product.Brand))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.UnitType.Abbreviation));

        CreateMap<InventoryEntry, InventoryEntryDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductBrand, opt => opt.MapFrom(s => s.Product.Brand))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.UnitType.Abbreviation));

        CreateMap<PriceSuggestion, PriceSuggestionDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store.Name))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.UnitType.Abbreviation))
            .ForMember(d => d.ObservedDate, opt => opt.MapFrom(s => s.ObservedDateUtc));
    }
}
