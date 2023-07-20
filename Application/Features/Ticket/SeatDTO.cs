using System.Text.Json.Serialization;

namespace Application.Features.Ticket
{
    public  class SeatDTO
    {
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        [JsonIgnore]
        public int AuditoriumId { get; set; }
    }
}
