using Application.DTOs.Products;
using MediatR;

namespace Application.Queries.Products
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public int Id { get; set; }
    }
}
