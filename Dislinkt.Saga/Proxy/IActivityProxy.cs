using Dislinkt.Saga.Data;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy
{
    public interface IActivityProxy
    {

        Task<(User, bool)> CreateActivity(User createduser);

        Task DeleteActivityAsync(User user);


    }
}
