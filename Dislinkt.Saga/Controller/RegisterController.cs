using Dislinkt.Saga.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Controller
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController:ControllerBase
    {

        private readonly IHttpClientFactory httpClientFactory;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [HttpPost]
        [Route("/register")]
        public async Task<bool> Register([FromBody] UserData user)
        {
            //profile 
            var request = JsonConvert.SerializeObject(user);
            var createdUserJson = new User();
          
                var profileClient = httpClientFactory.CreateClient("Profile");
                var profileResponse = await profileClient.PostAsync("Profile/register-user",
                    new StringContent(request, Encoding.UTF8, "application/JSON")
                    );
                var createdUser = await profileResponse.Content.ReadAsStringAsync();
                createdUserJson = JsonConvert.DeserializeObject<User>(createdUser);
           

            //connectionstry
              var nodeRequest = JsonConvert.SerializeObject(new ConnectionData { Id = createdUserJson.Id, UserName = createdUserJson.Username, Status = 1 });
                var connectionResponse = await profileClient.PostAsync("Connections/registerUser",
                    new StringContent(nodeRequest, Encoding.UTF8, "application/JSON")
                    );
               var isCreatedNode = await connectionResponse.Content.ReadAsStringAsync();

            if (isCreatedNode=="false")
            {
                await profileClient.DeleteAsync($"Profile/delete-user/{createdUserJson.Id}");
                return false;
            }
               
            
            //notifications
            var notificationRequest = JsonConvert.SerializeObject(new NotificationData
            {
                UserId = System.Guid.Parse(createdUserJson.Id),
                MessageOn = true,
                PostOn=true,
                JobOn=true,
                FriendRequestOn=true
            });
            var notificationResponse = await profileClient.PostAsync("Notifications/create-notification-settings", new StringContent(notificationRequest, Encoding.UTF8, "application/JSON"));

            //admin dashboard activity
            var activityRequest = JsonConvert.SerializeObject(new ActivityData
            {
                UserId = System.Guid.Parse(createdUserJson.Id),
                Text = "Sucessfully registered",
                Type = ActivityType.Registration,
                Date = DateTime.Now
            });
            var activityResponse = await profileClient.PostAsync("AdminDashboard/create-activity", new StringContent(activityRequest, Encoding.UTF8, "application/JSON"));


            return true;
        }
    }

  
}
