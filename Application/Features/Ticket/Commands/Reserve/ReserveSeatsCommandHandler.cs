using Application.Exceptions;
using Application.Features.Showtime;
using Domain.Auditorium;
using Domain.Showtime;
using Domain.Ticket;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Ticket.Commands.Reserve
{
    public class ReserveSeatsCommandHandler : IRequestHandler<ReserveSeatsCommand, ReservedTicketDTO>
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        public ReserveSeatsCommandHandler(ITicketsRepository ticketsRepository, IShowtimesRepository showtimesRepository, IAuditoriumsRepository auditoriumsRepository)
        {
            _ticketsRepository = ticketsRepository;
            _showtimesRepository = showtimesRepository;
            _auditoriumsRepository = auditoriumsRepository;
        }
        public async Task<ReservedTicketDTO> Handle(ReserveSeatsCommand request, CancellationToken cancellationToken)
        {

            var showtime = await _showtimesRepository.GetWithMoviesByIdAsync(request.Reservation.ShowtimeId, cancellationToken)
                ?? throw new NotFoundException(nameof(ShowtimeEntity), request.Reservation.ShowtimeId);

            var validator = new ReserveSeatsCommandValidator(_ticketsRepository, _showtimesRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
                throw new BadRequestException("The reservation cannot be completed.", validationResult);

            var auditorium = await _auditoriumsRepository.GetAsync(showtime.AuditoriumId, cancellationToken);           

            var seats = auditorium.Seats
                        .Where(seat => request.Reservation.Seats.Any(selectedSeat =>
                            selectedSeat.Row == seat.Row && selectedSeat.SeatNumber == seat.SeatNumber && showtime.AuditoriumId == seat.AuditoriumId))
                  .ToList();

            var ticket = await _ticketsRepository.CreateAsync(request.Reservation.ShowtimeId, seats, cancellationToken);

            return new ReservedTicketDTO
            {
                Id = ticket.Id,
                AuditoriumId = ticket.Showtime.AuditoriumId,
                Movie = new MovieDTO
                {
                    Id = ticket.Showtime.Movie.Id,
                    ImdbId = ticket.Showtime.Movie.ImdbId,
                    ReleaseDate = ticket.Showtime.Movie.ReleaseDate,
                    Stars = ticket.Showtime.Movie.Stars,
                    Title = ticket.Showtime.Movie.Title
                },
                NumberOfSeats = ticket.Seats.Count,
                Paid = ticket.Paid
            };
        }
    }
}
