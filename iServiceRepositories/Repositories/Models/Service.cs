using Microsoft.AspNetCore.Http;
using System.Security.Policy;

namespace iServiceRepositories.Repositories.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public int EstablishmentUserProfileId { get; set; }
        public int ServiceCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceNet { get; set; }
        public int EstimatedDuration { get; set; }
        public string? ServiceImage { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ServiceCategory? ServiceCategory { get; set; }
        public IFormFile? File { get; set; }
        public int? TotalPages { get; set; }
        public string? EstablishmentEmployeeIds { get; set; }
    }
}
