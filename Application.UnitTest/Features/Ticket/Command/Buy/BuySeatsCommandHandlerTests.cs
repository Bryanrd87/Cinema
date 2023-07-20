using Application.Exceptions;
using Domain.Auditorium;
using Domain.Seat;
using Domain.Showtime;
using Domain.Ticket;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;
using Application.Features.Ticket.Commands.Buy;
using AutoMapper;
using Application.MappingProfiles;
using System.Linq;

namespace Application.UnitTests.Features.Ticket.Command.Buy
{
    public class BuySeatsCommandHandlerTests
    {       
        private readonly Mock<ITicketsRepository> _ticketsRepositoryMock;
        private readonly IMapper _mapper;
        private readonly BuySeatsCommandHandler _handler;
        private ShowtimeEntity _showtimeEntity;

        public BuySeatsCommandHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MovieProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _ticketsRepositoryMock = new Mock<ITicketsRepository>();

            _handler = new BuySeatsCommandHandler(_ticketsRepositoryMock.Object, _mapper);

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
        public async Task Handle_Should_BuySeats()
        {
            //Arrange
            var ticketId = Guid.NewGuid();
            var command = new BuySeatsCommand()
            {
                Id = ticketId
            };

            IEnumerable<SeatEntity> seats = new List<SeatEntity> { new SeatEntity { Row = 1, SeatNumber = 1, AuditoriumId = 1 } };

            AuditoriumEntity expectedAuditorium = new AuditoriumEntity { Id = 1, Seats = seats.ToList() };

            var createdDate = DateTime.Now;

            var ticketEntity = new TicketEntity
            {
                Id = ticketId,
                CreatedTime = createdDate,
                Paid = false,
                Seats = seats.ToList(),
                ShowtimeId = 1,
                Showtime = _showtimeEntity
            };

            _ticketsRepositoryMock
            .Setup(repository => repository.GetAsync(ticketId, CancellationToken.None))
            .ReturnsAsync((Guid ticketId, CancellationToken cancellationToken) =>
            {
                return ticketEntity;
            });            

            _ticketsRepositoryMock
            .Setup(repository => repository.ConfirmPaymentAsync(ticketEntity, CancellationToken.None))
            .ReturnsAsync((TicketEntity ticketEntity, CancellationToken cancellationToken) =>
            {
                return new TicketEntity
                {
                    Id = ticketId,
                    CreatedTime = createdDate,
                    Paid = true,
                    Seats = seats.ToList(),
                    ShowtimeId = 1,
                    Showtime = _showtimeEntity
                };
            });

            //Act
            var result = await _handler.Handle(command, default);

            //Assert            
            result.ShouldNotBeNull();
            result.Id.ShouldBe(ticketId);
            result.Paid.ShouldBeTrue();
        }

        [Fact]
        public async Task Handle_Should_ReturnTicketNotFound()
        {
            //Arrange
            var ticketId = Guid.NewGuid();
            var command = new BuySeatsCommand()
            {
                Id = ticketId
            };

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"TicketEntity ({ticketId}) was not found.");

        }

        [Fact]
        public async Task Handle_Should_ReturnTicketAlreadyPaid()
        {
            //Arrange
            var ticketId = Guid.NewGuid();
            var command = new BuySeatsCommand()
            {
                Id = ticketId
            };

            IEnumerable<SeatEntity> seats = new List<SeatEntity> { new SeatEntity { Row = 1, SeatNumber = 1, AuditoriumId = 1 } };

            AuditoriumEntity expectedAuditorium = new AuditoriumEntity { Id = 1, Seats = seats.ToList() };

            _ticketsRepositoryMock
            .Setup(repository => repository.GetAsync(ticketId, CancellationToken.None))
            .ReturnsAsync((Guid ticketId, CancellationToken cancellationToken) =>
            {               
                return new TicketEntity
                {
                    Id = ticketId,
                    CreatedTime = DateTime.Now,
                    Paid = true,
                    Seats = seats.ToList(),
                    ShowtimeId = 1,
                    Showtime = _showtimeEntity
                };
            });

            // Act & Assert
            var exception = await Should.ThrowAsync<ReservationAlreadyPaidException>(async () => await _handler.Handle(command, CancellationToken.None));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"Reservation ({ticketId}) has already been paid.");
        }
    }
}
