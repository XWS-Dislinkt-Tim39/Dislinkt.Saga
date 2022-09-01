using Dislinkt.Saga.Data;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy.Implementation
{
    public class ConnectionProxy:IConnectionProxy
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ConnectionProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<(User, bool)> CreateNode(User createduser)
        {
            try
            {
                var profileClient = httpClientFactory.CreateClient("Profile");
                var nodeRequest = JsonConvert.SerializeObject(new ConnectionData { Id = createduser.Id, UserName = createduser.Username, Status = 1 });
               
                var connectionResponse = await profileClient.PostAsync("Connections/registerUser",
                        new StringContent(nodeRequest, Encoding.UTF8, "application/JSON")
                        );
                
                var isCreatedNode = await connectionResponse.Content.ReadAsStringAsync();

                if (isCreatedNode == "false")
                {
                    //await profileClient.DeleteAsync($"Profile/delete-user/{createduser.Id}");
                    Console.WriteLine("isCreatedNodeJob je false (ConnectionsProxy)");
                    return (null, false);
                }
                return (createduser, true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return (null, false);
            }

        }

        public async Task DeleteNodeAsync(User user)
        {
            var profileClient = httpClientFactory.CreateClient("Profile");

            await profileClient.DeleteAsync($"Connections/deleteUser/{user.Id}");

        }

    }
}
