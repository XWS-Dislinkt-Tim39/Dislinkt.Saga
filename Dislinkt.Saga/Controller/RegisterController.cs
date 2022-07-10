using Dislinkt.Saga.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var request = JsonConvert.SerializeObject(user);
            var profileClient = httpClientFactory.CreateClient("Profile");
            var profileResponse =await  profileClient.PostAsync("Profile/register-user",
                new StringContent(request,Encoding.UTF8,"application/JSON")
                );
            return true;
        }
    }

  
}
