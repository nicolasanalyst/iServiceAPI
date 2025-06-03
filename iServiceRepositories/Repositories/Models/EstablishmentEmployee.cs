using Microsoft.AspNetCore.Http;

namespace iServiceRepositories.Repositories.Models
{
    public class EstablishmentEmployee
    {
        public int EstablishmentEmployeeId { get; set; }
        public int EstablishmentUserProfileId { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? EmployeeImage { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IFormFile? File { get; set; }
        public bool? IsAvailable { get; set; }
        public int? AppointmentCount { get; set; }
    }
}
