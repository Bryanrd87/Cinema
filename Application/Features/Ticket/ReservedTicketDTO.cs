using Application.Features.Showtime;
using System;

namespace Application.Features.Ticket
{
    public class ReservedTicketDTO
    {
        public Guid Id { get; set; }
        public int NumberOfSeats { get; set; }
        public int AuditoriumId { get; set; }
        public MovieDTO Movie { get; set; }
        public bool Paid { get; set; }
    }
}
