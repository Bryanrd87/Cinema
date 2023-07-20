using Domain.Ticket;
using FluentValidation;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Domain.Seat;
using Domain.Showtime;
using System;
using System.Linq;

namespace Application.Features.Ticket.Commands.Reserve
{
    public class ReserveSeatsCommandValidator : AbstractValidator<ReserveSeatsCommand>
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IShowtimesRepository _showtimesRepository;
        private const int Span = 10;
        public ReserveSeatsCommandValidator(ITicketsRepository ticketsRepository, IShowtimesRepository showtimesRepository)
        {   
            RuleFor(q => q)
                .MustAsync(HasNotExistingReservation)
                .WithMessage("Seats have already been reserved within the last 10 minutes.");

            RuleFor(q => q)
               .MustAsync(AreNotSeatsSold)
               .WithMessage("Seats are already sold.");

            RuleFor(q => q)
               .Must(AreSeatsContiguous)
               .WithMessage("Seats must be contiguous.");

            _ticketsRepository = ticketsRepository;
            _showtimesRepository = showtimesRepository;
        }

        private async Task<bool> HasNotExistingReservation(ReserveSeatsCommand command, CancellationToken token)
        {      
            var result = await _ticketsRepository.HasNotExistingReservation(await SeatsList(command, token), TimeSpan.FromMinutes(Span), token);
            return result;
        }

        private async Task<bool> AreNotSeatsSold(ReserveSeatsCommand command, CancellationToken token)
        {         
            var result = await _ticketsRepository.AreNotSoldSeats(await SeatsList(command, token), token);
            return result;
        }

        private bool AreSeatsContiguous(ReserveSeatsCommand command)
        {            
            var sortedSeats = command.Reservation.Seats.OrderBy(s => s.Row).ThenBy(s => s.SeatNumber).ToList();
           
            for (var i = 1; i < sortedSeats.Count; i++)
            {
                if (sortedSeats[i].Row != sortedSeats[i - 1].Row || sortedSeats[i].SeatNumber != sortedSeats[i - 1].SeatNumber + 1)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<List<SeatEntity>> SeatsList(ReserveSeatsCommand command, CancellationToken token)
        {
            var showtime = await _showtimesRepository.GetWithTicketsByIdAsync(command.Reservation.ShowtimeId, token);
            var seatsList = new List<SeatEntity>();
            command.Reservation.Seats.ForEach(s =>
            {
                seatsList.Add(new SeatEntity
                {
                    Row = s.Row,
                    SeatNumber = s.SeatNumber,
                    AuditoriumId = showtime.AuditoriumId
                });
            });

            return seatsList;
        }
    }
}
