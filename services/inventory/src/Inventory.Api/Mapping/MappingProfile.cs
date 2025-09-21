using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;

namespace Inventory.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReserveRequestDto, Reservation>()
    .ForMember(d => d.Id, opt => opt.Ignore())
    .ForMember(d => d.Status, opt => opt.MapFrom(_ => ReservationStatus.Reserved))
    .ForMember(d => d.StockItemId, opt => opt.Ignore()) // заполняется сервисом
    .ForMember(d => d.ExpiresAt, opt => opt.Ignore());   // задаётся сервисом

        }
    }
}
