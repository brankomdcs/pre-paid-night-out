using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetHistoryRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:8080/api/transaction/history?userId={user.Id}";
        }
    }
}
