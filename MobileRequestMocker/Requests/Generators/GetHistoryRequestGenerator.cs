using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetHistoryRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"{Configuration.GetInstance().ReverseProxyUrl}/Services/Orchestrator/api/transaction/history?userId={user.Id}";
        }
    }
}
