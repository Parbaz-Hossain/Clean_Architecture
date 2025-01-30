using Application.DTOs.Products;
using Domain.Interfaces.Products;
using MediatR;

namespace Application.Queries.Products
{
    public class GetAllProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetAllProductsAsync();
            return products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price });
        }
    }
}
