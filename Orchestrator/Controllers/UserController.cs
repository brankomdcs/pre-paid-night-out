using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;
using Orchestrator.Util;
using Orchestrator.Customizations;

namespace Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PpnoHttpClient ppnoHttpClient;
        private readonly Uri accountServiceName;

        public UserController(HttpClient httpClient, StatelessServiceContext serviceContext)
        {
            ppnoHttpClient = new PpnoHttpClient(httpClient);
            accountServiceName = Orchestrator.GetAccountServiceNameFrom(serviceContext);
        }

        [HttpPost("{register}")]
        public async Task<IActionResult> Register(int id, string account, string name, Enums.UserType type)
        {
            string postUrl = $"{Orchestrator.GetAccountServiceAddressFrom(accountServiceName)}/api/User/register" +
                             $"?id={id}&account={account}&name={name}&type={type}&PartitionKey={UserPartitionKeyGenerator.GenerateFor(id)}&PartitionKind=Int64Range";
            
            return await ppnoHttpClient.PostAsync(postUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            string getUrl = $"{Orchestrator.GetAccountServiceAddressFrom(accountServiceName)}/api/User" +
                            $"?id={id}&PartitionKey={UserPartitionKeyGenerator.GenerateFor(id)}&PartitionKind=Int64Range";
            
            return await ppnoHttpClient.GetAsync(getUrl);
        }

    }
}
