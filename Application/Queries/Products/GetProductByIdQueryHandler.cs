using Application.DTOs.Products;
using AutoMapper;
using Domain.Interfaces.Products;
using MediatR;

namespace Application.Queries.Products
{
    public class GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper) : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductByIdAsync(request.Id);
            var productDto = mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}
