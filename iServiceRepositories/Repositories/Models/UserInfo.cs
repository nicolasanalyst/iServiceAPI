namespace iServiceRepositories.Repositories.Models
{
    public class UserInfo
    {
        public User User { get; set; }
        public UserRole UserRole { get; set; }
        public UserProfile? UserProfile { get; set; }
        public Address? Address { get; set; }
        public string? Token { get; set; }
    }
}
