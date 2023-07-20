using System.Collections.Generic;

namespace Application.Features.Ticket
{
    public class ReservationDTO
    {
        public int ShowtimeId { get; set; }
        public List<SeatDTO> Seats { get; set; }
    }
}
