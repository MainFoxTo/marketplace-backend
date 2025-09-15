using AutoMapper;
using Catalog.Api.DTOs;
using Catalog.Domain.Entities;

namespace Catalog.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<ProductForCreateDto, Product>();
            CreateMap<Product, ProductDto>();

            // ProductVersion mappings
            CreateMap<ProductVersion, ProductVersionDto>();
            CreateMap<ProductVersionDto, ProductVersion>();

            // For updates - ignore Id and dates
            CreateMap<ProductForCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
