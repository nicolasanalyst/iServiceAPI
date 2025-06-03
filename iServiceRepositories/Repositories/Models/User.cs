using Mysqlx.Crud;
using System;

namespace iServiceRepositories.Repositories.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
