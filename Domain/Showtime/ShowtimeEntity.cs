﻿using System;
using System.Collections.Generic;
using Domain.Movie;
using Domain.Ticket;

namespace Domain.Showtime
{
    public class ShowtimeEntity
    {
        public int Id { get; set; }        
        public MovieEntity Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        public ICollection<TicketEntity> Tickets { get; set; }
    }
}
