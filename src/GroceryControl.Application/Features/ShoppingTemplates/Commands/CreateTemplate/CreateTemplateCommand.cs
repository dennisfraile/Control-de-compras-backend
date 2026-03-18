using GroceryControl.Application.Features.ShoppingTemplates.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.ShoppingTemplates.Commands.CreateTemplate;

public record CreateTemplateItemInput(Guid ProductId, decimal Quantity, int UnitTypeId);

public record CreateTemplateCommand(string Name, List<CreateTemplateItemInput> Items) : IRequest<ShoppingTemplateDto>;
