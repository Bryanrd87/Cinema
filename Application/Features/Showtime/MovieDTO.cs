using System;

namespace Application.Features.Showtime
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
