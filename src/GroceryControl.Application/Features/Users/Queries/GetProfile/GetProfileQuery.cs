using GroceryControl.Application.Features.Users.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Users.Queries.GetProfile;

public record GetProfileQuery : IRequest<UserProfileDto>;
