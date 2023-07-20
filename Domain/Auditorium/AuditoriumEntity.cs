using System.Collections.Generic;
using Domain.Seat;
using Domain.Showtime;

namespace Domain.Auditorium
{
    public class AuditoriumEntity
    {
        public int Id { get; set; }
        public List<ShowtimeEntity> Showtimes { get; set; }
        public ICollection<SeatEntity> Seats { get; set; }

    }
}
