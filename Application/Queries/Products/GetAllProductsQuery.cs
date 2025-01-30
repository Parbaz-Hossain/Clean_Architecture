using Application.DTOs.Products;
using MediatR;

namespace Application.Queries.Products
{
    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>> { }
}
