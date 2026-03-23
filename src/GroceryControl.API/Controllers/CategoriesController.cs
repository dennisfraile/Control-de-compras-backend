using GroceryControl.Application.Common.Interfaces;
using GroceryControl.Application.Features.Categories.DTOs;
using GroceryControl.Application.Features.Categories.Queries.GetCategories;
using GroceryControl.Domain.Entities;
using GroceryControl.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

public record CreateCategoryRequest(string Name);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public CategoriesController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var maxId = _context.Categories.Max(c => c.Id);
        var category = new Category
        {
            Id = maxId + 1,
            Name = request.Name,
            DefaultUnitCategory = UnitCategory.Countable,
            SortOrder = maxId + 1,
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(ct);

        return Ok(new CategoryDto(category.Id, category.Name, null, null, category.DefaultUnitCategory, category.SortOrder));
    }
}
