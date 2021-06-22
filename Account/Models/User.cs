namespace Account.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public Enums.UserType Type { get; set; }
    }
}
