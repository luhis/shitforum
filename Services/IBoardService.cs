using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IBoardService
    {
        Task<IReadOnlyList<Board>> GetAll(CancellationToken cancellationToken);
    }
}
