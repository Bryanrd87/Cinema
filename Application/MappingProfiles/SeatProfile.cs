using Application.Features.Ticket;
using AutoMapper;
using Domain.Seat;

namespace Application.MappingProfiles
{
    public class SeatProfile : Profile
    {
        public SeatProfile()
        { 
            CreateMap<SeatDTO, SeatEntity>().ReverseMap();
        }
    }
}
