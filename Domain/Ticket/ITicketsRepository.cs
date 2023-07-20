using Domain.Seat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Ticket
{
    public interface ITicketsRepository
    {
        Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel);
        Task<TicketEntity> CreateAsync(int showtimeId, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel);
        Task<TicketEntity> GetAsync(Guid id, CancellationToken cancel);
        Task<IEnumerable<TicketEntity>> GetEnrichedAsync(int showtimeId, CancellationToken cancel);
        Task<bool> HasNotExistingReservation(List<SeatEntity> seats, TimeSpan timeSpan, CancellationToken cancel);
        Task<bool> AreNotSoldSeats(List<SeatEntity> seats,  CancellationToken cancel);
        Task<IEnumerable<TicketEntity>> GetAllAsync(CancellationToken cancel);
    }
}