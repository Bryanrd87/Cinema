using System.Threading.Tasks;
using System.Threading;
using Domain.Auditorium;
using Microsoft.EntityFrameworkCore;
using Domain.Showtime;
using Domain.Ticket;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Domain.Movie;

namespace Application.Data
{
    public interface ICinemaContext
    {
        DbSet<AuditoriumEntity> Auditoriums { get; set; }

        DbSet<ShowtimeEntity> Showtimes { get; set; }

        DbSet<MovieEntity> Movies { get; set; }

        DbSet<TicketEntity> Tickets { get; set; }

        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
