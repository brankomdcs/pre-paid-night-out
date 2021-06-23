using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Models;

namespace Transaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private const string transactionCollectionName = "transactions";
        private readonly IReliableStateManager stateManager;

        public TransactionController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        [HttpPost("{create}")]
        public async Task<IActionResult> Create(int userId, string accountTo, decimal amount)
        {
            IReliableDictionary<Guid, PaymentTransaction> usersCollection = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, PaymentTransaction>>(transactionCollectionName);

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                await usersCollection.AddAsync(tx, Guid.NewGuid(), new PaymentTransaction { UserId = userId, AccountTo = accountTo, Amount = amount, Time = DateTime.Now });
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            CancellationToken ct = new CancellationToken();
            IReliableDictionary<Guid, PaymentTransaction> usersCollection = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, PaymentTransaction>>(transactionCollectionName);

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, PaymentTransaction>> list = await usersCollection.CreateEnumerableAsync(tx);
                Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, PaymentTransaction>> enumerator = list.GetAsyncEnumerator();

                List<KeyValuePair<Guid, PaymentTransaction>> result = new List<KeyValuePair<Guid, PaymentTransaction>>();

                while (await enumerator.MoveNextAsync(ct))
                {
                    if (enumerator.Current.Value.UserId == userId)
                        result.Add(enumerator.Current);
                }

                return new JsonResult(result.OrderBy(transaction => transaction.Value.Time));
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            CancellationToken ct = new CancellationToken();
            IReliableDictionary<Guid, PaymentTransaction> usersCollection = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, PaymentTransaction>>(transactionCollectionName);

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, PaymentTransaction>> list = await usersCollection.CreateEnumerableAsync(tx);
                Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, PaymentTransaction>> enumerator = list.GetAsyncEnumerator();

                decimal result = 0;

                while (await enumerator.MoveNextAsync(ct))
                {
                    if (enumerator.Current.Value.UserId == userId)
                        result += enumerator.Current.Value.Amount;
                }

                return new JsonResult(new { total = result });
            }
        }
    }
}
