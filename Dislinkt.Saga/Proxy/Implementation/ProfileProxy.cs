using Dislinkt.Saga.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy.Implementation
{
    public class ProfileProxy:IProfileProxy
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProfileProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<(User,bool)> CreateUser(UserData user)
        {
            try
            {
                var request = JsonConvert.SerializeObject(user);
                var createdUserJson = new User();

                var profileClient = httpClientFactory.CreateClient("Profile");
                var profileResponse = await profileClient.PostAsync("Profile/register-user",
                        new StringContent(request, Encoding.UTF8, "application/JSON")
                        );
                var createdUser = await profileResponse.Content.ReadAsStringAsync();
                createdUserJson = JsonConvert.DeserializeObject<User>(createdUser);
                return (createdUserJson, true);
            }
            catch
            {
                return (null, false);
            }

        }

        public async Task DeleteProfileAsync(User user)
        {
            var profileClient = httpClientFactory.CreateClient("Profile");

            await profileClient.DeleteAsync($"Profile/delete-user/{user.Id}");

        }
    }
}
