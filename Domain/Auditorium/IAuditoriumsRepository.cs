using System.Threading;
using System.Threading.Tasks;

namespace Domain.Auditorium
{
    public interface IAuditoriumsRepository
    {
        Task<AuditoriumEntity> GetAsync(int auditoriumId, CancellationToken cancel);
    }
}