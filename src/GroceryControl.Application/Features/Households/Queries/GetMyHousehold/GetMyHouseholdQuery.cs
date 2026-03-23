using GroceryControl.Application.Features.Households.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Households.Queries.GetMyHousehold;

public record GetMyHouseholdQuery() : IRequest<HouseholdDto?>;
