using Microsoft.AspNetCore.Mvc;
using Orchestrator.Constants;
using Orchestrator.Customizations;
using Orchestrator.Models;
using Orchestrator.Util;
using System;
using System.Fabric;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PpnoHttpClient ppnoHttpClient;
        private readonly Uri transactionServiceName;
        private readonly Uri paymentServiceName;

        public TransactionController(HttpClient httpClient, StatelessServiceContext serviceContext)
        {
            ppnoHttpClient = new PpnoHttpClient(httpClient);
            transactionServiceName = Orchestrator.GetTransactionServiceNameFrom(serviceContext);
            paymentServiceName = Orchestrator.GetPaymentServiceNameFrom(serviceContext);
        }

        [HttpPost("{charge}")]
        public async Task<IActionResult> Charge(int userId, string accountTo, decimal amount)
        {
            // Check if user has enough credit:
            string getBalanceUrl = $"{Orchestrator.GetTransactionServiceAddressFrom(transactionServiceName)}/api/Transaction/balance?userId={userId}" +
                                   $"&PartitionKey={TransactionPartitionKeyGenerator.GenerateFor(userId)}&PartitionKind=Int64Range";
            
            var balance = await ppnoHttpClient.GetDeserializedResponse<Balance>(getBalanceUrl);
            
            if (balance.Total < amount)
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = $"{{ 'error' : 'Non-sufficient funds'}}"
                };

            // Charge organization for the purchase amount:
            string paymentUrl = $"{Orchestrator.GetPaymentServiceAddressFrom(paymentServiceName)}/api/Payment/charge" +
                                $"?accountFrom={Accounts.PPNO_ORGANIZATION_ACCOUNT}&accountTo={accountTo}&amount={amount}";
            
            await ppnoHttpClient.PostAsync(paymentUrl);

            // Deduct from users credit by creating negative amount transaction:
            string transactionUrl = $"{Orchestrator.GetAccountServiceAddressFrom(transactionServiceName)}/api/Transaction/create?userId={userId}&accountTo={accountTo}&amount={-amount}" +
                                    $"&PartitionKey={TransactionPartitionKeyGenerator.GenerateFor(userId)}&PartitionKind=Int64Range";

            return await ppnoHttpClient.PostAsync(transactionUrl);
        }

        [HttpGet("history")]
        public async Task<IActionResult> Get(int userId)
        { 
            string getUrl = $"{Orchestrator.GetTransactionServiceAddressFrom(transactionServiceName)}/api/Transaction/history" +
                            $"?userId={userId}&PartitionKey={TransactionPartitionKeyGenerator.GenerateFor(userId)}&PartitionKind=Int64Range";
            
            return await ppnoHttpClient.GetAsync(getUrl);
        }

        [HttpGet("{balance}")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            string getUrl = $"{Orchestrator.GetTransactionServiceAddressFrom(transactionServiceName)}/api/Transaction/balance" +
                            $"?userId={userId}&PartitionKey={TransactionPartitionKeyGenerator.GenerateFor(userId)}&PartitionKind=Int64Range";

            return await ppnoHttpClient.GetAsync(getUrl);
        }
    }
}
