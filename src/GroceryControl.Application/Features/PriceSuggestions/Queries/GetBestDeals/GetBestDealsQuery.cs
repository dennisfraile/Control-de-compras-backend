using GroceryControl.Application.Features.PriceSuggestions.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.PriceSuggestions.Queries.GetBestDeals;

public record GetBestDealsQuery(int? CategoryId, int Limit = 20) : IRequest<List<PriceSuggestionDto>>;
