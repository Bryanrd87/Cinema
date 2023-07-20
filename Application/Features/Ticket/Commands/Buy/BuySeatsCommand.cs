using MediatR;
using System;

namespace Application.Features.Ticket.Commands.Buy
{
    public class BuySeatsCommand : IRequest<ReservedTicketDTO>
    {
        public Guid Id { get; set; }
    }
}
