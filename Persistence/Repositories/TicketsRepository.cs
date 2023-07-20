using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Domain.Ticket;
using Domain.Seat;

namespace Persistence.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly CinemaContext _context;

        public TicketsRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<TicketEntity> GetAsync(Guid id, CancellationToken cancel)
        {
            return await _context.Tickets
                .Include(x => x.Showtime)
                .Include(x => x.Showtime.Movie)
                .Include(x => x.Seats)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<IEnumerable<TicketEntity>> GetEnrichedAsync(int showtimeId, CancellationToken cancel)
        {
            return await _context.Tickets
                .Include(x => x.Showtime)
                .Include(x => x.Seats)
                .Where(x => x.ShowtimeId == showtimeId)
                .ToListAsync(cancel);
        }

        public async Task<TicketEntity> CreateAsync(int showtimeId, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel)
        {
            var ticket = await _context.Tickets.AddAsync(new TicketEntity
            {               
                ShowtimeId = showtimeId,
                Seats = new List<SeatEntity>(selectedSeats),
            });

            await _context.SaveChangesAsync(cancel);

            return ticket.Entity;
        }

        public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
        {
            ticket.Paid = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync(cancel);
            return ticket;
        }

        public async Task<bool> HasNotExistingReservation(List<SeatEntity> seats, TimeSpan timeSpan, CancellationToken cancel)
        {
            var lastReservationTime = DateTime.Now - timeSpan;

            var tickets = await _context.Tickets
                        .Include(x => x.Seats)              
                        .ToListAsync(cancel);
            var result = tickets
                .Where(ticket => ticket.CreatedTime > lastReservationTime)               
                .Any(ticket => seats.All(s => ticket.Seats.Any(ts => ts.Row == s.Row && ts.SeatNumber == s.SeatNumber))) == false;

            return result;
        }

        public async Task<bool> AreNotSoldSeats(List<SeatEntity> seats, CancellationToken cancel)
        {
            var tickets = await _context.Tickets
                        .Include(x => x.Seats)
                        .ToListAsync(cancel);
            return tickets
                .Where(ticket => ticket.Paid)                 
                .Any(ticket =>  seats.All(s => ticket.Seats.Any(ts => ts.Row == s.Row && ts.SeatNumber == s.SeatNumber))) == false;
        }

        public async Task<IEnumerable<TicketEntity>> GetAllAsync(CancellationToken cancel)
        {
            return await _context.Tickets
                .Include(x => x.Showtime)              
                .Include(x => x.Showtime.Movie)
                .Include(x => x.Seats)
                .ToListAsync(cancel);
        }
    }
}
