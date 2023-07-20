using MediatR;

namespace Application.Features.Ticket.Commands.Reserve
{
    public class ReserveSeatsCommand : IRequest<ReservedTicketDTO>
    {
        public ReservationDTO Reservation { get; set; }
    }
}
