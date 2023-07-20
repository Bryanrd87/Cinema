using Application.Features.Showtime;
using Domain.Ticket;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsListQueryHandler : IRequestHandler<GetTicketsListQuery, List<ReservedTicketDTO>>
    {
        private readonly ITicketsRepository _ticketsRepository;
        public GetTicketsListQueryHandler(ITicketsRepository ticketsRepository)
        {
            _ticketsRepository = ticketsRepository;
        }

        public async Task<List<ReservedTicketDTO>> Handle(GetTicketsListQuery request, CancellationToken cancellationToken)
        {            
            var tickets = await _ticketsRepository.GetAllAsync(cancellationToken);

            var reservedTicketList = new List<ReservedTicketDTO>();

            tickets.ToList().ForEach(ticket =>
            {
                reservedTicketList.Add( new ReservedTicketDTO
                {
                    Id = ticket.Id,
                    AuditoriumId = ticket.Showtime.AuditoriumId,
                    Movie = new MovieDTO
                    {
                        Id = ticket.Showtime.Movie.Id,
                        ImdbId = ticket.Showtime.Movie.ImdbId,
                        ReleaseDate = ticket.Showtime.Movie.ReleaseDate,
                        Stars = ticket.Showtime.Movie.Stars,
                        Title = ticket.Showtime.Movie.Title
                    },
                    NumberOfSeats = ticket.Seats.Count,
                    Paid = ticket.Paid
                });
            });

            return reservedTicketList;
        }
    }
}
