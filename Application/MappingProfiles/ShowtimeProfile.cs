using Application.Features.Showtime;
using AutoMapper;
using Domain.Showtime;

namespace Application.MappingProfiles
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            CreateMap<ShowtimeDTO, ShowtimeEntity>().ReverseMap();
            CreateMap<ShowtimeDetailsDTO, ShowtimeEntity>().ReverseMap();
        }
    }
}
    