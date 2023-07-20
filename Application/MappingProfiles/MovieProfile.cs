using Application.Features.Showtime;
using AutoMapper;
using Domain.Movie;

namespace Application.MappingProfiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieDTO, MovieEntity>().ReverseMap();           
        }
    }
}
