using GroceryControl.Application.Features.ShoppingTemplates.Commands.CreateTemplate;
using GroceryControl.Application.Features.ShoppingTemplates.Commands.DeleteTemplate;
using GroceryControl.Application.Features.ShoppingTemplates.DTOs;
using GroceryControl.Application.Features.ShoppingTemplates.Queries.GetTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/shopping-templates")]
[Authorize]
public class ShoppingTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShoppingTemplateDto>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTemplatesQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingTemplateDto>> Create(
        [FromBody] CreateTemplateCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTemplateCommand(id), ct);
        return NoContent();
    }
}
