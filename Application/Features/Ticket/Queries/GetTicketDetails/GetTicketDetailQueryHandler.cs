using Application.Exceptions;
using Application.Features.Showtime;
using AutoMapper;
using Domain.Ticket;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Ticket.Queries.GetTicketDetails
{
    public class GetTicketDetailQueryHandler : IRequestHandler<GetTicketDetailQuery, ReservedTicketDTO>
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IMapper _mapper;
        public GetTicketDetailQueryHandler(ITicketsRepository ticketsRepository, IMapper mapper)
        {
            _ticketsRepository = ticketsRepository;
            _mapper = mapper;
        }
        public async Task<ReservedTicketDTO> Handle(GetTicketDetailQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketsRepository.GetAsync(request.Id, cancellationToken);
            return ticket == null
                ? throw new NotFoundException(nameof(ticket), request.Id)
                : new ReservedTicketDTO
                {
                    Id = ticket.Id,
                    AuditoriumId = ticket.Showtime.AuditoriumId,
                    Movie = _mapper.Map<MovieDTO>(ticket.Showtime.Movie),
                    NumberOfSeats = ticket.Seats.Count(),
                    Paid = ticket.Paid
                };
        }
    }
}
