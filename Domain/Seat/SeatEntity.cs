using Domain.Auditorium;
using Domain.Ticket;
using System;

namespace Domain.Seat
{
    public class SeatEntity
    {      
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public AuditoriumEntity Auditorium { get; set; }
        public Guid TicketId { get; set; }       
        public TicketEntity Ticket { get; set; } 
    }
}
