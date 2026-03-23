using GroceryControl.Application.Features.Households.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Households.Commands.CreateHousehold;

public record CreateHouseholdCommand(string Name) : IRequest<HouseholdDto>;
