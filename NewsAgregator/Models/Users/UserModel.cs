namespace NewsAgregator.Mvc.Models.Users
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
