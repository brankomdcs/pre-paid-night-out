using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetBalanceRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:19081/Services/Orchestrator/api/transaction/balance?userId={user.Id}";
        }
    }
}
