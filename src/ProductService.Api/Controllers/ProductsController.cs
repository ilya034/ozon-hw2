using Microsoft.AspNetCore.Mvc;
using ProductService.Api.Contracts;
using ProductService.Api.Mapping;
using ProductService.Application.Abstractions;
using ProductService.Application.Models;
using ProductService.Domain.Products;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public sealed class ProductsController(IProductAppService appService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatedProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedProductResponse>> Create(
        [FromBody] CreateProductHttpRequest request,
        CancellationToken cancellationToken)
    {
        var id = await appService.CreateAsync(
            new CreateProductRequest(request.Name, request.Price, request.Weight, request.Type, request.WarehouseId),
            cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new CreatedProductResponse(id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductHttpResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await appService
            .GetByIdAsync(new GetProductRequest(id), cancellationToken)
            .ConfigureAwait(false);

        return Ok(ToResponse(product));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedProductsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedProductsResponse>> List(
        [FromQuery] DateTime? createdFrom,
        [FromQuery] DateTime? createdTo,
        [FromQuery] ProductType? type,
        [FromQuery] long? warehouseId,
        [FromQuery] int page = ProductMapping.DefaultPage,
        [FromQuery] int pageSize = ProductMapping.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var request = new ListProductsRequest(createdFrom, createdTo, type, warehouseId, page, pageSize);
        var result = await appService.ListAsync(request, cancellationToken).ConfigureAwait(false);

        return Ok(new PagedProductsResponse(
            result.Items.Select(ToResponse).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages));
    }

    private static ProductHttpResponse ToResponse(Product product) => new(
        product.Id,
        product.Name,
        product.Price,
        product.Weight,
        product.Type,
        product.CreatedAt,
        product.WarehouseId);
}
