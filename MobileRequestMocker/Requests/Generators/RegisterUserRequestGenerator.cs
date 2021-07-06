using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class RegisterUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"{Configuration.GetInstance().ReverseProxyUrl}/Services/Orchestrator/api/user/register?id={user.Id}&account={user.Account}&name={user.Name}&type={user.Type}";
        }
    }
}
