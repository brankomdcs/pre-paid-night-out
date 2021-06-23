using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:19081/Services/Orchestrator/api/user?id={user.Id}";
        }
    }
}
