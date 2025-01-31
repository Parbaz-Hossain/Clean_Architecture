using Application.Commands.Products;
using Application.DTOs.Products;
using AutoMapper;
using Domain.Entities.Products;

namespace Application.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateProductCommand, Product>();
            CreateMap<Product, ProductDto>();
        }
    }
}
