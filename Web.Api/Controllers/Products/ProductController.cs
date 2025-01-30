using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Queries.Products;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers.Products
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/product")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
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
