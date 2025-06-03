namespace iServiceRepositories.Repositories.Models
{
    public class ServiceEmployee
    {
        public int ServiceEmployeeId { get; set; }
        public int ServiceId { get; set; }
        public int EstablishmentEmployeeId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
