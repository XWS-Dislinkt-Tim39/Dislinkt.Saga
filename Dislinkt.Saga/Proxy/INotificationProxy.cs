using Dislinkt.Saga.Data;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy
{
    public interface INotificationProxy
    {
        Task<(User, bool)> CreateNotificationSetting(User createduser);

        Task DeleteNotificationAsync(User user);
    }
}
