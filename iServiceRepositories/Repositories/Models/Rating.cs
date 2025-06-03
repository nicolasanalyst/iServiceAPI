using System.Numerics;

namespace iServiceRepositories.Repositories.Models
{
    public class Rating
    {
        public double Value { get; set; }
        public int Total { get; set; }
        public List<Feedback> Feedback { get; set; }
    }
}