using Dislinkt.Saga.Data;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy
{
    public interface IJobsProxy
    {
        Task<(User, bool)> CreateNode(User user);

        Task DeleteNodeAsync(User user);
    }
}
