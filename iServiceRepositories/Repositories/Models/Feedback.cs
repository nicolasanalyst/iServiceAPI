namespace iServiceRepositories.Repositories.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int AppointmentId { get; set; }
        public string? Description { get; set; }
        public double Rating { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string? Name { get; set; }
    }
}
