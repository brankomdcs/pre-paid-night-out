namespace Orchestrator.Models
{
    public class User
    {
        public User(int id, string account, string name, Enums.UserType type)
        {
            Id = id;
            Account = account;
            Name = name;
            Type = type;
        }

        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public Enums.UserType Type { get; set; }
    }
}
