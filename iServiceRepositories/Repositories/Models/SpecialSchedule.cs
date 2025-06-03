namespace iServiceRepositories.Repositories.Models
{
    public class SpecialSchedule
    {
        public int SpecialScheduleId { get; set; }
        public int EstablishmentUserProfileId { get; set; }
        public DateTime Date { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string? BreakStart { get; set; }
        public string? BreakEnd { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
