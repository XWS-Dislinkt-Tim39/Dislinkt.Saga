using Dislinkt.Saga.Data;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy.Implementation
{
    public class ActivityProxy:IActivityProxy
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ActivityProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<(User, bool)> CreateActivity(User createdUser)
        {
            try
            {
                var profileClient = httpClientFactory.CreateClient("Profile");
                var activityRequest = JsonConvert.SerializeObject(new ActivityData
                {
                    UserId = System.Guid.Parse(createdUser.Id),
                    Text = "Sucessfully registered",
                    Type = ActivityType.Registration,
                    Date = DateTime.Now
                });
                var activityResponse = await profileClient.PostAsync("AdminDashboard/create-activity", new StringContent(activityRequest, Encoding.UTF8, "application/JSON"));
                var isCreatedActivity = await activityResponse.Content.ReadAsStringAsync();

                if (isCreatedActivity == "false")
                {
                    return (null, false);
                }
                return (createdUser, true);
            }
            catch
            {
                return (null, false);
            }
        }

        public async Task DeleteActivityAsync(User user)
        {
            var profileClient = httpClientFactory.CreateClient("Profile");

            await profileClient.DeleteAsync($"Connections/deleteUser/{user.Id}");

        }
    }
}
