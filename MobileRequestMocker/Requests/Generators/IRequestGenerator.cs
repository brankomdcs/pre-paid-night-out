using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public interface IRequestGenerator
    {
        public string GenerateFor(User user);
    }
}
