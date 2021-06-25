using Microsoft.AspNetCore.Mvc;
using Orchestrator.Constants;
using Orchestrator.Customizations;
using Orchestrator.Util;
using System;
using System.Fabric;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly PpnoHttpClient ppnoHttpClient;
        private readonly Uri transactionServiceName;
        private readonly Uri paymentServiceName;

        public CreditController(HttpClient httpClient, StatelessServiceContext serviceContext)
        {
            ppnoHttpClient = new PpnoHttpClient(httpClient);
            transactionServiceName = Orchestrator.GetTransactionServiceNameFrom(serviceContext);
            paymentServiceName = Orchestrator.GetPaymentServiceNameFrom(serviceContext);
        }

        [HttpPost("{add}")]
        public async Task<IActionResult> Add(int userId, string userAccount, decimal amount)
        {
            Orchestrator.RegisterRequestForMetrics();
            // Charge user for the amount of credit:
            string paymentUrl = $"{Orchestrator.GetPaymentServiceAddressFrom(paymentServiceName)}/api/Payment/charge?" +
                                $"accountFrom={userAccount}&accountTo={Accounts.PPNO_ORGANIZATION_ACCOUNT}&amount={amount}";

            await ppnoHttpClient.PostAsync(paymentUrl);

            // Create positive amount transaction to increase users credit:
            string transactionUrl = $"{Orchestrator.GetAccountServiceAddressFrom(transactionServiceName)}/api/Transaction/create" +
                                    $"?userId={userId}&accountTo={Accounts.PPNO_ORGANIZATION_ACCOUNT}&amount={amount}" +
                                    $"&PartitionKey={TransactionPartitionKeyGenerator.GenerateFor(userId)}&PartitionKind=Int64Range";

            return await ppnoHttpClient.PostAsync(transactionUrl);
        }
    }
}
