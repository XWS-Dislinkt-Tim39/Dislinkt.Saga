using Dislinkt.Saga.Data;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Proxy.Implementation
{
    public class JobsProxy : IJobsProxy
    {
        private readonly IHttpClientFactory httpClientFactory;

        public JobsProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;


        }

        public async Task<(User, bool)> CreateNode(User user)
        {
            try
            {
                var profileClient = httpClientFactory.CreateClient("Profile");
                var nodeRequestJob = JsonConvert.SerializeObject(new ConnectionJobData { Id = user.Id, Username = user.Username, Seniority = user.Seniority });

                var connectionResponseJob = await profileClient.PostAsync("Jobs/addUser",
                       new StringContent(nodeRequestJob, Encoding.UTF8, "application/JSON"));

                var isCreatedNodeJob = await connectionResponseJob.Content.ReadAsStringAsync();

                if (isCreatedNodeJob == "false")
                {
                    Console.WriteLine("isCreatedNodeJob je false (JobsProxy)");
                    return (null, false);
                }
                return (user, true);
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

            await profileClient.DeleteAsync($"Jobs/deleteUser/{user.Id}");

        }
    }
}
