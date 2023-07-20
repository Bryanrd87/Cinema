using MediatR;
using System;

namespace Application.Features.Ticket.Queries.GetTicketDetails
{
    public class GetTicketDetailQuery : IRequest<ReservedTicketDTO>
    {
        public Guid Id { get; set; }
    }
}
