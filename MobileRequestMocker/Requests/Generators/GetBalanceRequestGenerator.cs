using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetBalanceRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"{Configuration.GetInstance().ReverseProxyUrl}/Services/Orchestrator/api/transaction/balance?userId={user.Id}";
        }
    }
}
