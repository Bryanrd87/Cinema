using Domain.Showtime;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.IntegrationTests.Repositories
{
    public class ShowtimesRepositoryIntegrationTests : IDisposable
    {
        private readonly DbContextOptions<CinemaContext> _options;
        private readonly CinemaContext _context;
        private readonly ShowtimesRepository _repository;

        public ShowtimesRepositoryIntegrationTests()
        {           
            var databaseName = Guid.NewGuid().ToString();
           
            _options = new DbContextOptionsBuilder<CinemaContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            
            _context = new CinemaContext(_options);
          
            _repository = new ShowtimesRepository(_context);
        }

        [Fact]
        public async Task CreateShowtime_WithValidData_ShouldAddShowtimeToDatabase()
        {
            // Arrange
            var showtimeEntity = new ShowtimeEntity { Id = 1 };

            // Act
            var result = await _repository.CreateShowtime(showtimeEntity, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(showtimeEntity.Id);
           
            var showtimeInDatabase = await _context.Showtimes.FindAsync(showtimeEntity.Id);
            showtimeInDatabase.ShouldNotBeNull();
            showtimeInDatabase.Id.ShouldBe(showtimeEntity.Id);
        }
       

        public void Dispose()
        {           
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
