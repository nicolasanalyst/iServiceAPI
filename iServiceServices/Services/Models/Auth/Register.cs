using iServiceRepositories.Repositories.Models;

namespace iServiceServices.Services.Models.Auth
{
    public class Register
    {
        public UserProfile UserProfile { get; set; }
        public Address? Address { get; set; }
    }
}
