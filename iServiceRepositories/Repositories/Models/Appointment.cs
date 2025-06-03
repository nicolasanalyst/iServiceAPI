namespace iServiceRepositories.Repositories.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int ServiceId { get; set; }
        public int ClientUserProfileId { get; set; }
        public int EstablishmentUserProfileId { get; set; }
        public AppointmentStatusEnum AppointmentStatusId { get; set; }
        public int EstablishmentEmployeeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public UserInfo? ClientUserInfo { get; set; }
        public UserInfo? EstablishmentUserInfo { get; set; }
        public EstablishmentEmployee? EstablishmentEmployee { get; set; }
        public Service? Service { get; set; }
        public Feedback? Feedback { get; set; }
    }
}
