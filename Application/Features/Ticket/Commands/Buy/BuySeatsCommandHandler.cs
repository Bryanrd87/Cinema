using Application.Exceptions;
using Application.Features.Showtime;
using AutoMapper;
using Domain.Ticket;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Ticket.Commands.Buy
{
    public class BuySeatsCommandHandler : IRequestHandler<BuySeatsCommand, ReservedTicketDTO>
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IMapper _mapper;
        public BuySeatsCommandHandler(ITicketsRepository ticketsRepository, IMapper mapper)
        {
            _ticketsRepository = ticketsRepository;
            _mapper = mapper;
        }
        public async Task<ReservedTicketDTO> Handle(BuySeatsCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketsRepository.GetAsync(request.Id, cancellationToken) 
                ?? throw new NotFoundException(nameof(TicketEntity), request.Id);           

            if (ticket.Paid)
                throw new ReservationAlreadyPaidException(request.Id);

            var ticketPaid = await _ticketsRepository.ConfirmPaymentAsync(ticket, cancellationToken);

            return new ReservedTicketDTO
            {
                Id = ticketPaid.Id,
                AuditoriumId = ticketPaid.Showtime.AuditoriumId,
                Movie = _mapper.Map<MovieDTO>(ticketPaid.Showtime.Movie),
                NumberOfSeats = ticketPaid.Seats.Count,
                Paid = ticketPaid.Paid
            };
        }
    }
}
