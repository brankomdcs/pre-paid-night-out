using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetHistoryRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:19081/Services/Orchestrator/api/transaction/history?userId={user.Id}";
        }
    }
}
