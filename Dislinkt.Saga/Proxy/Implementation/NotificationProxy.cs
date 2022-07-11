using Dislinkt.Saga.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy.Implementation
{
    public class NotificationProxy:INotificationProxy
    {
        private readonly IHttpClientFactory httpClientFactory;

        public NotificationProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<(User, bool)> CreateNotificationSetting(User createduser)
        {
            try
            {
                var profileClient = httpClientFactory.CreateClient("Profile");
                var notificationRequest = JsonConvert.SerializeObject(new NotificationData
                {
                    UserId = System.Guid.Parse(createduser.Id),
                    MessageOn = true,
                    PostOn = true,
                    JobOn = true,
                    FriendRequestOn = true
                });
                var notificationResponse = await profileClient.PostAsync("Notifications/create-notification-settings", new StringContent(notificationRequest, Encoding.UTF8, "application/JSON"));
                var isCreatedNotification = await notificationResponse.Content.ReadAsStringAsync();

                if (isCreatedNotification == "false")
                {
                    return (null, false);
                }
                return (createduser, true);
            }
            catch
            {
                return (null, false);
            }


        }

        public async Task DeleteNotificationAsync(User user)
        {
            var profileClient = httpClientFactory.CreateClient("Profile");

            await profileClient.DeleteAsync($"Notifications/delete-by-userId/{user.Id}");

        }
    }
}
