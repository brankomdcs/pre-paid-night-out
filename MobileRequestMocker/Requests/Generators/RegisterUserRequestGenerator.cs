using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class RegisterUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:8080/api/user/register?id={user.Id}&account={user.Account}&name={user.Name}&type={user.Type}";
        }
    }
}
