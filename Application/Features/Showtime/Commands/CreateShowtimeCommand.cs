using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Application.Features.Showtime.Commands
{
    public class CreateShowtimeCommand : IRequest<int>
    {
        public string MovieId { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        [JsonIgnore]
        public MovieDTO Movie { get; set; }
    }
}
