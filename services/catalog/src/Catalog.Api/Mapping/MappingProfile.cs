using AutoMapper;
using Catalog.Api.DTOs;
using Catalog.Domain.Entities;

namespace Catalog.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product: при создании/обновлении не перезаписываем системные поля
            CreateMap<ProductForCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Product, ProductDto>();

            // Version mappings
            CreateMap<ProductVersionForCreateDto, ProductVersion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()); // задаём явно в контроллере

            CreateMap<ProductVersion, ProductVersionDto>();
        }
    }
}
