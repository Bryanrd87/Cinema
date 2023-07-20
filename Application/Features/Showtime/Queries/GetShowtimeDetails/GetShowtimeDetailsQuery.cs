using MediatR;

namespace Application.Features.Showtime.Queries.GetShowtimeDetails
{
    public class GetShowtimeDetailsQuery : IRequest<ShowtimeDetailsDTO>
    {
        public int Id { get; set; }
    }
}
