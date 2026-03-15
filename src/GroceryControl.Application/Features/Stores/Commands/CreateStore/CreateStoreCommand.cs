using GroceryControl.Application.Features.Stores.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Stores.Commands.CreateStore;

public record CreateStoreCommand(
    string Name,
    string? Branch,
    string? Address,
    string? City) : IRequest<StoreDto>;
