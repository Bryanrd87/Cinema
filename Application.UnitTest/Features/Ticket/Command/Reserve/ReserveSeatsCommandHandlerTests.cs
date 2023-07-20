using Application.Exceptions;
using Application.Features.Ticket;
using Application.Features.Ticket.Commands.Reserve;
using Domain.Auditorium;
using Domain.Seat;
using Domain.Showtime;
using Domain.Ticket;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Features.Ticket.Command.Reserve
{
    public class ReserveSeatsCommandHandlerTests
    {
        private readonly Mock<IShowtimesRepository> _showtimesRepositoryMock;
        private readonly Mock<ITicketsRepository> _ticketsRepositoryMock;
        private readonly Mock<IAuditoriumsRepository> _auditoriumsRepositoryMock;
        private readonly ReserveSeatsCommandHandler _handler;
        private ShowtimeEntity _showtimeEntity;

        public ReserveSeatsCommandHandlerTests()
        {
            _showtimesRepositoryMock = new Mock<IShowtimesRepository>();
            _ticketsRepositoryMock = new Mock<ITicketsRepository>();
            _auditoriumsRepositoryMock = new Mock<IAuditoriumsRepository>();

            _handler = new ReserveSeatsCommandHandler(_ticketsRepositoryMock.Object,
            _showtimesRepositoryMock.Object, _auditoriumsRepositoryMock.Object
             );

            //Arrange
            _showtimeEntity = new ShowtimeEntity
            {
                Id = 1,
                AuditoriumId = 1,
                SessionDate = DateTime.Now.AddMinutes(-11),
                Movie = new Domain.Movie.MovieEntity
                {
                    Id = 1,
                    ImdbId = "tt0071562",
                    ReleaseDate = new DateTime(1974, 05, 23),
                    Stars = "Francis Ford Coppola (dir.), Al Pacino, Robert De Niro",
                    Title = "The Godfather Part II"
                }
            };

        }

        [Fact]
        public async Task Handle_Should_ReserveSeats()
        {
            //Arrange
            var command = new ReserveSeatsCommand()
            {
                Reservation = new ReservationDTO
                {
                    ShowtimeId = 1,
                    Seats = new List<SeatDTO> { new SeatDTO { Row = 1, SeatNumber = 1 } }
                }
            };

            _ticketsRepositoryMock
               .Setup(repo => repo.HasNotExistingReservation(It.IsAny<List<SeatEntity>>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);

            _showtimesRepositoryMock
            .Setup(repo => repo.GetWithMoviesByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
           {
               return _showtimeEntity;
           });

            _showtimesRepositoryMock
                .Setup(repo => repo.GetWithTicketsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
            {
                return _showtimeEntity;
            });

            _ticketsRepositoryMock
                .Setup(repo => repo.AreNotSoldSeats(It.IsAny<List<SeatEntity>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            IEnumerable<SeatEntity> seats = new List<SeatEntity> { new SeatEntity { Row = 1, SeatNumber = 1, AuditoriumId = 1 } };

            AuditoriumEntity expectedAuditorium = new AuditoriumEntity { Id = 1, Seats = seats.ToList() };

            _auditoriumsRepositoryMock
                .Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAuditorium);

            _ticketsRepositoryMock
            .Setup(repository => repository.CreateAsync(1, seats, CancellationToken.None))
            .ReturnsAsync((int ticketId, List<SeatEntity> seatsList, CancellationToken cancellationToken) =>
            {
                seatsList = seats.ToList();
                return new TicketEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    Paid = false,
                    Seats = seatsList,
                    ShowtimeId = 1,
                    Showtime = _showtimeEntity
                };
            });

            //Act
            var result = await _handler.Handle(command, default);

            //Assert            
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task Handle_Should_ReturnShowtimeNotFound()
        {
            //Arrange
            var showtimeId = 1;
            var command = new ReserveSeatsCommand()
            {
                Reservation = new ReservationDTO
                {
                    ShowtimeId = showtimeId,
                    Seats = new List<SeatDTO> { new SeatDTO { Row = 1, SeatNumber = 1 } }
                }
            };

            //Act
            var exception = await Should.ThrowAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));

            //Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"ShowtimeEntity ({showtimeId}) was not found.");
        }

        [Fact]
        public async Task Handle_Should_NotReserveSeats_SoldSeats()
        {
            //Arrange
            var command = new ReserveSeatsCommand()
            {
                Reservation = new ReservationDTO
                {
                    ShowtimeId = 1,
                    Seats = new List<SeatDTO> { new SeatDTO { Row = 1, SeatNumber = 1 } }
                }
            };

            _ticketsRepositoryMock
               .Setup(repo => repo.HasNotExistingReservation(It.IsAny<List<SeatEntity>>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);

            _showtimesRepositoryMock
            .Setup(repo => repo.GetWithMoviesByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
           {
               return _showtimeEntity;
           });

            _showtimesRepositoryMock
                .Setup(repo => repo.GetWithTicketsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
            {
                return _showtimeEntity;
            });

            _ticketsRepositoryMock
                .Setup(repo => repo.AreNotSoldSeats(It.IsAny<List<SeatEntity>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

            //Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("The reservation cannot be completed.");

            string[] errorMessages = exception.ValidationErrors[""];
            errorMessages.ShouldNotBeNull();
            errorMessages.ShouldNotBeEmpty();
            errorMessages[0].ShouldBe("Seats are already sold.");
        }

        [Fact]
        public async Task Handle_Should_NotReserveSeats_SeatsReservedWithinTenMinutes()
        {
            //Arrange
            var command = new ReserveSeatsCommand()
            {
                Reservation = new ReservationDTO
                {
                    ShowtimeId = 1,
                    Seats = new List<SeatDTO> { new SeatDTO { Row = 1, SeatNumber = 1 } }
                }
            };

            _ticketsRepositoryMock
               .Setup(repo => repo.HasNotExistingReservation(It.IsAny<List<SeatEntity>>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(false);

            _showtimesRepositoryMock
            .Setup(repo => repo.GetWithMoviesByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
           {
               return _showtimeEntity;
           });

            _showtimesRepositoryMock
                .Setup(repo => repo.GetWithTicketsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
            {
                return _showtimeEntity;
            });

            _ticketsRepositoryMock
                .Setup(repo => repo.AreNotSoldSeats(It.IsAny<List<SeatEntity>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act 
            var exception = await Should.ThrowAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

            //Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("The reservation cannot be completed.");

            string[] errorMessages = exception.ValidationErrors[""];
            errorMessages.ShouldNotBeNull();
            errorMessages.ShouldNotBeEmpty();
            errorMessages[0].ShouldBe("Seats have already been reserved within the last 10 minutes.");
        }

        [Fact]
        public async Task Handle_Should_NotReserveSeats_SeatsAreNotContiguos()
        {
            //Arrange
            var command = new ReserveSeatsCommand()
            {
                Reservation = new ReservationDTO
                {
                    ShowtimeId = 1,
                    Seats = new List<SeatDTO> { new SeatDTO { Row = 1, SeatNumber = 1 }, new SeatDTO { Row = 1, SeatNumber = 3 } }
                }
            };

            _ticketsRepositoryMock
               .Setup(repo => repo.HasNotExistingReservation(It.IsAny<List<SeatEntity>>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);

            _showtimesRepositoryMock
            .Setup(repo => repo.GetWithMoviesByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
           {
               return _showtimeEntity;
           });

            _showtimesRepositoryMock
                .Setup(repo => repo.GetWithTicketsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int showtimeId, CancellationToken cancellationToken) =>
            {
                return _showtimeEntity;
            });

            _ticketsRepositoryMock
                .Setup(repo => repo.AreNotSoldSeats(It.IsAny<List<SeatEntity>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

            //Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("The reservation cannot be completed.");

            string[] errorMessages = exception.ValidationErrors[""];
            errorMessages.ShouldNotBeNull();
            errorMessages.ShouldNotBeEmpty();
            errorMessages[0].ShouldBe("Seats must be contiguous.");
        }

    }
}
