using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.PriceSuggestions.Commands.SubmitPrice;

public record SubmitPriceCommand(
    Guid ProductId,
    Guid StoreId,
    decimal Price,
    decimal Quantity,
    int UnitTypeId,
    DateTime ObservedDateUtc) : IRequest<PriceSuggestionDto>;
