using GroceryControl.Application.Common.Exceptions;
using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Users.DTOs;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<UserProfile> _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IRepository<UserProfile> profileRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(UserProfile), _currentUser.UserId);

        profile.PurchaseFrequency = request.PurchaseFrequency;
        profile.HouseholdSize = request.HouseholdSize;
        profile.PreferredCurrency = request.PreferredCurrency;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserProfileDto(
            profile.Id,
            profile.UserId,
            profile.PurchaseFrequency,
            profile.HouseholdSize,
            profile.PreferredCurrency,
            profile.UpdatedAtUtc);
    }
}
