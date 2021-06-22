using Account.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string userCollectionName = "users";
        private readonly IReliableStateManager stateManager;

        public UserController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        [HttpPost("{register}")]
        public async Task<IActionResult> Register(int id, string account, string name, Enums.UserType type)
        {
            IReliableDictionary<int, User> usersCollection = await stateManager.GetOrAddAsync<IReliableDictionary<int, User>>(userCollectionName);

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                await usersCollection.AddAsync(tx, id, new User { Id = id, Account = account, Name = name, Type = type });
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            CancellationToken ct = new CancellationToken();
            IReliableDictionary<int, User> usersCollection = await stateManager.GetOrAddAsync<IReliableDictionary<int, User>>(userCollectionName);

            using (ITransaction tx = stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<int, User>> list = await usersCollection.CreateEnumerableAsync(tx);
                Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<int, User>> enumerator = list.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    if (enumerator.Current.Key == id)
                        return new JsonResult(enumerator.Current.Value);
                }

                return new NotFoundResult();
            }
        }

    }
}
