using MediatR;
using System.Collections.Generic;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsListQuery : IRequest<List<ReservedTicketDTO>>
    {
    }
}
