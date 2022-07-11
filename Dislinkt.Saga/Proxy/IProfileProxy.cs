using Dislinkt.Saga.Data;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy
{
    public interface IProfileProxy
    {
        Task<(User, bool)> CreateUser(UserData user);

        Task DeleteProfileAsync(User user);
    }
}
