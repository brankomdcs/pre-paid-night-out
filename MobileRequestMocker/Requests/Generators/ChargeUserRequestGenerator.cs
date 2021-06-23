using MobileRequestMocker.Models;
using System;

namespace MobileRequestMocker.Requests.Generators
{
    public class ChargeUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            Random random = new Random();
            int randomAmount = random.Next(50, 100);
            return $"http://localhost:19081/Services/Orchestrator/api/transaction/charge?userId={user.Id}&accountTo={user.Account}&amount={randomAmount}";
        }
    }
}
