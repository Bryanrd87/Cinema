using Application.MappingProfiles;
using AutoMapper;
using Domain.Ticket;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Shouldly;
using Application.Exceptions;
using Domain.Seat;
using Domain.Showtime;
using System.Linq;
using Application.Features.Ticket.Queries.GetTicketDetails;

namespace Application.UnitTests.Features.Ticket.Queries.GetTicketDetails
{
    public class GetTicketDetailsQueryHandlerTests
    {
        private readonly Mock<ITicketsRepository> _ticketsRepositoryMock;
        private readonly IMapper _mapper;
        private readonly GetTicketDetailQueryHandler _handler;
        public GetTicketDetailsQueryHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MovieProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _ticketsRepositoryMock = new Mock<ITicketsRepository>();

            _handler = new GetTicketDetailQueryHandler(_ticketsRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound()
        {
            //Arrange
            var query = new GetTicketDetailQuery { Id = Guid.NewGuid() };

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(async () => await _handler.Handle(query, CancellationToken.None));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"ticket ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Handle_Should_Return_TicketReserved()
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
            var query = new GetTicketDetailQuery { Id = ticketId };
            var cancellationToken = CancellationToken.None;

            _ticketsRepositoryMock.Setup(r => r.GetAsync(ticketId, cancellationToken)).ReturnsAsync(ticket);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(ticketId);
        }
    }
}
