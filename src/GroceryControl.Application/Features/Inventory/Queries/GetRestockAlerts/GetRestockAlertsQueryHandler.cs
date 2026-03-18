using GroceryControl.Application.Common.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Inventory.Queries.GetRestockAlerts;

public class GetRestockAlertsQueryHandler : IRequestHandler<GetRestockAlertsQuery, List<RestockAlert>>
{
    private readonly IConsumptionAnalyzer _consumptionAnalyzer;
    private readonly ICurrentUserService _currentUser;

    public GetRestockAlertsQueryHandler(IConsumptionAnalyzer consumptionAnalyzer, ICurrentUserService currentUser)
    {
        _consumptionAnalyzer = consumptionAnalyzer;
        _currentUser = currentUser;
    }

    public async Task<List<RestockAlert>> Handle(GetRestockAlertsQuery request, CancellationToken cancellationToken)
    {
        return await _consumptionAnalyzer.GetRestockAlertsAsync(_currentUser.UserId, cancellationToken);
    }
}
