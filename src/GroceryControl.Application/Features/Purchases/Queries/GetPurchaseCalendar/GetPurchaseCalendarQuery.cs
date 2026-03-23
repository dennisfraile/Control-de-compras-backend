using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.GetPurchaseCalendar;

public record GetPurchaseCalendarQuery(int Year, int Month) : IRequest<List<PurchaseCalendarDayDto>>;
