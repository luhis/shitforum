using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Setup;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IShitForumDbConfig
    {
        string DbLocation { get; }
    }
}
