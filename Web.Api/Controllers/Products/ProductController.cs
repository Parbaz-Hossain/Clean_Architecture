using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Queries.Products;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers.Products
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/product")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
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
                var products = await mediator.Send(new GetProductByIdQuery { Id = id });
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
