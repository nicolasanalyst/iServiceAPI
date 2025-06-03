namespace iServiceRepositories.Repositories.Models
{
    public class ServiceCategory
    {
        public int ServiceCategoryId { get; set; }
        public int EstablishmentUserProfileId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
