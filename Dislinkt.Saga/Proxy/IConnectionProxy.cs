using Dislinkt.Saga.Data;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy
{
    public interface IConnectionProxy
    {
        Task<(User, bool)> CreateNode(User createduser);

        Task DeleteNodeAsync(User user);
    }
}
