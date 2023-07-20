using System;

namespace Application.Features.Showtime
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public MovieDTO Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }   
    }
}
