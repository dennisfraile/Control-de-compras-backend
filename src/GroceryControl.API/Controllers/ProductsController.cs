using GroceryControl.Application.Features.Products.Commands.CreateProduct;
using GroceryControl.Application.Features.Products.Commands.UpdateProduct;
using GroceryControl.Application.Features.Products.DTOs;
using GroceryControl.Application.Features.Products.Queries.GetProducts;
using GroceryControl.Application.Features.Products.Commands.ToggleFavorite;
using GroceryControl.Application.Features.Products.Queries.GetFavorites;
using GroceryControl.Application.Features.Products.Queries.SearchProducts;
using GroceryControl.Application.Features.Purchases.DTOs;
using GroceryControl.Application.Features.Purchases.Queries.GetPriceHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(
        [FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductsQuery(categoryId, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductDto>>> Search(
        [FromQuery] string q, CancellationToken ct)
    {
        var result = await _mediator.Send(new SearchProductsQuery(q), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id, [FromBody] UpdateProductCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/toggle-favorite")]
    public async Task<ActionResult<bool>> ToggleFavorite(Guid id, CancellationToken ct)
    {
        var isFavorited = await _mediator.Send(new ToggleFavoriteCommand(id), ct);
        return Ok(new { isFavorited });
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<List<ProductDto>>> GetFavorites(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFavoritesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/price-history")]
    public async Task<ActionResult<List<PriceHistoryDto>>> GetPriceHistory(
        Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPriceHistoryQuery(id), ct);
        return Ok(result);
    }
}
