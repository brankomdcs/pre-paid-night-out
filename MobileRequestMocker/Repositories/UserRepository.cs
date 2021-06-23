using System.Collections.Generic;

namespace MobileRequestMocker.Repositories
{
    public class UserRepository
    {
        public List<Models.User> Users { get; }
        public UserRepository()
        {
            Users = new List<Models.User>() {
                new Models.User { Id = 1, Account = "125883", Name = "Branko" },
                new Models.User { Id = 2, Account = "225883", Name = "Marko" },
                new Models.User { Id = 3, Account = "325883", Name = "Zarko" },
                new Models.User { Id = 4, Account = "425883", Name = "Darko" },
                new Models.User { Id = 5, Account = "525883", Name = "Janko" }
            };
        }
    }
}
