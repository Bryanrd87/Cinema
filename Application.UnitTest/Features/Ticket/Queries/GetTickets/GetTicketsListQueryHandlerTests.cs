using Application.Features.Ticket.Queries.GetTickets;
using Domain.Seat;
using Domain.Showtime;
using Domain.Ticket;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Linq;
using Shouldly;

namespace Application.UnitTests.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsListQueryHandlerTests
    {
        private readonly Mock<ITicketsRepository> _ticketsRepositoryMock;
        private readonly GetTicketsListQueryHandler _handler;
        public GetTicketsListQueryHandlerTests()
        {
            _ticketsRepositoryMock = new Mock<ITicketsRepository>();

            _handler = new GetTicketsListQueryHandler(_ticketsRepositoryMock.Object);
        }

        public async Task Handle_Should_ReturnTicketList()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            IEnumerable<SeatEntity> seats = new List<SeatEntity> { new SeatEntity { Row = 1, SeatNumber = 1, AuditoriumId = 1 } };
            var showtimeEntity = new ShowtimeEntity
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
            var ticket = new TicketEntity
            {
                Id = ticketId,
                Showtime = showtimeEntity,
                Seats = seats.ToList(),
                Paid = false
            };
            var query = new GetTicketsListQuery();
            var cancellationToken = CancellationToken.None;

            _ticketsRepositoryMock.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(new List<TicketEntity> { ticket });

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }
    }
}
