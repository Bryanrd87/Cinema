using Application.Exceptions;
using AutoMapper;
using Domain.Showtime;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Showtime.Queries.GetShowtimeDetails
{
    public class GetShowtimeDetailsRequestHandler : IRequestHandler<GetShowtimeDetailsQuery, ShowtimeDetailsDTO>
    {
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;

        public GetShowtimeDetailsRequestHandler(IShowtimesRepository showtimesRepository, IMapper mapper)
        {
            _showtimesRepository = showtimesRepository;
            _mapper = mapper;
        }

        public async Task<ShowtimeDetailsDTO> Handle(GetShowtimeDetailsQuery request, CancellationToken cancellationToken)
        {
            var showtime = await _showtimesRepository.GetWithMoviesByIdAsync(request.Id, cancellationToken);
            return showtime == null
                ? throw new NotFoundException(nameof(ShowtimeEntity), request.Id)
                : _mapper.Map<ShowtimeDetailsDTO>(showtime);
        }
    }
}
