using System.Threading.Tasks;
using System.Threading;

namespace Domain.Movie
{
    public interface IMovieRepository
    {
        Task<MovieEntity> CreateMovie(MovieEntity movieEntity, CancellationToken cancel);
    }
}
