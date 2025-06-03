using Mysqlx.Crud;
using System;

namespace iServiceRepositories.Repositories.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public int? EstablishmentCategoryId { get; set; } // Para estabelecimentos
        public int? AddressId { get; set; } // Opcional para clientes
        public string Document { get; set; }
        public DateTime? DateOfBirth { get; set; } // Opcional para não clientes
        public string? Phone { get; set; }
        public string? CommercialName { get; set; } // Opcional para não estabelecimentos
        public string? CommercialPhone { get; set; } // Opcional para não estabelecimentos
        public string? CommercialEmail { get; set; } // Opcional para não estabelecimentos
        public string? Description { get; set; } // Opcional para estabelecimentos
        public string? ProfileImage { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public Rating? Rating { get; set; }
        public Schedule? Schedule { get; set; }
    }
}
