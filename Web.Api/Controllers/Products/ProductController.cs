using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Queries.Products;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Web.Api.Controllers.Products
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/product")]
    [ApiController]
    public class ProductController(IMediator mediator, IDistributedCache cache, ILogger<ProductController> logger) : ControllerBase
    {
        [MapToApiVersion("1.0")]
        [HttpGet("get-all-products")]
        public async Task<ActionResult<List<ProductDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await mediator.Send(new GetAllProductsQuery());
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [MapToApiVersion("2.0")]
        [HttpGet("{id}")]        
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var cachedProductJson = await cache.GetStringAsync($"product:{id}");
                if(cachedProductJson is not null)
                {
                    logger.LogInformation($"Product {id} retrieved from cache.");
                    var cachedProduct = JsonSerializer.Deserialize<ProductDto>(cachedProductJson);
                    return Ok(cachedProduct);
                }

                var products = await mediator.Send(new GetProductByIdQuery { Id = id });
                if (products is null)
                    return NotFound();

                var productJson = JsonSerializer.Serialize(products);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache for 10 minutes
                    SlidingExpiration = TimeSpan.FromMinutes(5) // Cache expire if no activity for 5 minutes
                };

                await cache.SetStringAsync($"product:{id}", productJson, cacheOptions);
                logger.LogInformation($"Product {id} retrieved from database and stored in cache.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [MapToApiVersion("1.0")]
        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand createProductCommand)
        {
            try
            {
                var productId = await mediator.Send(createProductCommand);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
