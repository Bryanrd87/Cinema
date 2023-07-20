using Domain.Movie;
using Domain.Showtime;
using Domain.Ticket;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Repositories
{
    public class ShowtimesRepositoryTests
    {
        [Fact]
        public async Task GetWithMoviesByIdAsync_WithValidId_ShouldReturnShowtimeWithMovies()
        {
            // Arrange
            int showtimeId = 1;
            var showtimes = new List<ShowtimeEntity>
            {
                new ShowtimeEntity { Id = 1, Movie = new MovieEntity { Id = 1, Title = "Movie 1" }, AuditoriumId = 1 },
                new ShowtimeEntity { Id = 2, Movie = new MovieEntity { Id = 2, Title = "Movie 2" }, AuditoriumId = 1 }
            };

            var options = new DbContextOptionsBuilder<CinemaContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CinemaContext(options);
            context.Database.EnsureCreated();
            context.Showtimes.AddRange(showtimes);
            context.SaveChanges();

            var repository = new ShowtimesRepository(context);

            // Act
            var result = await repository.GetWithMoviesByIdAsync(showtimeId, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(showtimeId);
            result.Movie.ShouldNotBeNull();
            result.Movie.Id.ShouldBe(showtimeId);
            result.Movie.Title.ShouldBe($"Movie {showtimeId}");
        }

        [Fact]
        public async Task GetWithTicketsByIdAsync_WithValidId_ShouldReturnShowtimeWithTickets()
        {
            // Arrange
            int showtimeId = 1;
            var ticketIdOne = Guid.NewGuid();
            var ticketIdTwo = Guid.NewGuid();
            var showtimes = new List<ShowtimeEntity>
            {
                new ShowtimeEntity { Id = 1, AuditoriumId = 1, Tickets = new List<TicketEntity> { new TicketEntity { Id = ticketIdOne }, new TicketEntity { Id = ticketIdTwo } } },
                new ShowtimeEntity { Id = 2, AuditoriumId = 1, Tickets = new List<TicketEntity> { new TicketEntity { Id = Guid.NewGuid() }, new TicketEntity { Id = Guid.NewGuid() } } }
            };

            var options = new DbContextOptionsBuilder<CinemaContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase1")
                .Options;

            using var context = new CinemaContext(options);
            context.Database.EnsureCreated();
            context.Showtimes.AddRange(showtimes);
            context.SaveChanges();

            var repository = new ShowtimesRepository(context);

            // Act
            var result = await repository.GetWithTicketsByIdAsync(showtimeId, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(showtimeId);
            result.Tickets.ShouldNotBeNull();
            result.Tickets.Count.ShouldBe(2);
            result.Tickets.Select(t => t.Id).ShouldContain(ticketIdOne);
            result.Tickets.Select(t => t.Id).ShouldContain(ticketIdTwo);
        }

        [Fact]
        public async Task GetAllAsync_WithFilter_ShouldReturnFilteredShowtimes()
        {
            // Arrange
            var showtimes = new List<ShowtimeEntity>
            {
                new ShowtimeEntity { Id = 1, AuditoriumId = 1, Movie = new MovieEntity { Id = 1, Title = "Movie 1" } },
                new ShowtimeEntity { Id = 2, AuditoriumId = 1, Movie = new MovieEntity { Id = 2, Title = "Movie 2" } },
                new ShowtimeEntity { Id = 3, AuditoriumId = 1, Movie = new MovieEntity { Id = 3, Title = "Movie 3" } }
            };

            var options = new DbContextOptionsBuilder<CinemaContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            using var context = new CinemaContext(options);
            context.Database.EnsureCreated();
            context.Showtimes.AddRange(showtimes);
            context.SaveChanges();

            var repository = new ShowtimesRepository(context);

            // Act
            var result = await repository.GetAllAsync(st => st.Id > 1, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.Select(st => st.Id).ShouldContain(2);
            result.Select(st => st.Id).ShouldContain(3);
        }

        [Fact]
        public async Task CreateShowtime_WithValidData_ShouldAddShowtimeToDatabase()
        {
            // Arrange
            var showtimeEntity = new ShowtimeEntity { Id = 1, AuditoriumId = 1 };

            var options = new DbContextOptionsBuilder<CinemaContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            using var context = new CinemaContext(options);
            context.Database.EnsureCreated();
            var repository = new ShowtimesRepository(context);

            // Act
            var result = await repository.CreateShowtime(showtimeEntity, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(showtimeEntity.Id);
            context.Showtimes.Count().ShouldBe(1);
            context.Showtimes.First().Id.ShouldBe(showtimeEntity.Id);
        }
    }
}
