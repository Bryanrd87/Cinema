using MediatR;
using System.Collections.Generic;

namespace Application.Features.Showtime.Queries.GetShowtimes
{
    public class GetShowtimeListQuery : IRequest<List<ShowtimeDTO>>
    {
    }
}
