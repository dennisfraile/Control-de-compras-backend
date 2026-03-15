using GroceryControl.Application.Features.Stores.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Stores.Queries.GetStores;

public record GetStoresQuery : IRequest<List<StoreDto>>;
