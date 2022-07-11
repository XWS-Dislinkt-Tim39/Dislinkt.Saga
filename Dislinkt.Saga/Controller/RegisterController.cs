using Dislinkt.Saga.Data;
using Dislinkt.Saga.Menager;
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

        private readonly IRegistrationMenager _registrationMenager;

        public RegisterController(IRegistrationMenager registrationMenager)
        {
           this._registrationMenager= registrationMenager;
        }
        [HttpPost]
        [Route("/register")]
        public bool Register([FromBody] UserData user)
        {
            return _registrationMenager.Register(user);

            //profile 
          /*  var request = JsonConvert.SerializeObject(user);
            var createdUserJson = new User();
          
            var profileClient = httpClientFactory.CreateClient("Profile");
            var profileResponse = await profileClient.PostAsync("Profile/register-user",
                    new StringContent(request, Encoding.UTF8, "application/JSON")
                    );
            var createdUser = await profileResponse.Content.ReadAsStringAsync();
            createdUserJson = JsonConvert.DeserializeObject<User>(createdUser);
           

            //connection
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
            var isCreatedNotification = await notificationResponse.Content.ReadAsStringAsync();

            if (isCreatedNotification == "false")
            {
                await profileClient.DeleteAsync($"Profile/delete-user/{createdUserJson.Id}");
                await profileClient.DeleteAsync($"Connections/deleteUser/{createdUserJson.Id}");
                return false;
            }

            //admin dashboard activity
            var activityRequest = JsonConvert.SerializeObject(new ActivityData
            {
                UserId = System.Guid.Parse(createdUserJson.Id),
                Text = "Sucessfully registered",
                Type = ActivityType.Registration,
                Date = DateTime.Now
            });
            var activityResponse = await profileClient.PostAsync("AdminDashboard/create-activity", new StringContent(activityRequest, Encoding.UTF8, "application/JSON"));
            var isCreatedActivity = await activityResponse.Content.ReadAsStringAsync();

            if (isCreatedActivity == "false")
            {
                await profileClient.DeleteAsync($"Profile/delete-user/{createdUserJson.Id}");
                await profileClient.DeleteAsync($"Connections/deleteUser/{createdUserJson.Id}");
                await profileClient.DeleteAsync($"Notifications/delete-by-userId/{createdUserJson.Id}");
                return false;
            }

            return true;*/
        }
    }

  
}
