using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetUserRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:8080/api/user?id={user.Id}";
        }
    }
}
