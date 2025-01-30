using AutoMapper;
using Domain.Entities.Products;
using Domain.Interfaces.Products;
using MediatR;

namespace Application.Commands.Products
{
    public class CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper) : IRequestHandler<CreateProductCommand, int>
    {
        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = mapper.Map<Product>(request);
            await productRepository.AddProductAsync(product);
            return product.Id;
        }
    }
}
