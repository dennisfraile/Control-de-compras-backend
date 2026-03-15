using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.PriceSuggestions.Queries.GetLowestPrices;

public record GetLowestPricesQuery(Guid ProductId) : IRequest<List<PriceSuggestionDto>>;
