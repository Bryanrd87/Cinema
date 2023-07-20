using AutoMapper;
using Domain.Showtime;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Showtime.Queries.GetShowtimes
{
    public class GetShowtimeListHandler : IRequestHandler<GetShowtimeListQuery, List<ShowtimeDTO>>
    {
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;
        public GetShowtimeListHandler(IShowtimesRepository showtimesRepository,
             IMapper mapper)
        {
            _showtimesRepository = showtimesRepository;
            _mapper = mapper;
        }
        public async Task<List<ShowtimeDTO>> Handle(GetShowtimeListQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ShowtimeEntity, bool>> filter = null;
            var showtimes = await _showtimesRepository.GetAllAsync(filter, cancellationToken);

            return _mapper.Map<List<ShowtimeDTO>>(showtimes);
        }
    }
}
