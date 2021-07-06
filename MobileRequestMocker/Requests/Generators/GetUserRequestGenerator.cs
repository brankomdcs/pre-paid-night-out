using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"{Configuration.GetInstance().ReverseProxyUrl}/Services/Orchestrator/api/user?id={user.Id}";
        }
    }
}
