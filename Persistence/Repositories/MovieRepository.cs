using System.Threading.Tasks;
using System.Threading;
using Domain.Movie;

namespace Persistence.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly CinemaContext _context;

        public MovieRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<MovieEntity> CreateMovie(MovieEntity movieEntity, CancellationToken cancel)
        {
            var movie = await _context.Movies.AddAsync(movieEntity, cancel);
            await _context.SaveChangesAsync(cancel);
            return movie.Entity;
        }
    }
}
