namespace JobScoreServer.Models
{
    public class JobDescription
    {
        public int Id { get; set; }
        public int JobId { get; set; } = 0;
        public int UserId { get; set; } 
        public string Title { get; set; } 
        public string Content { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public decimal Score { get; set; }
        public User User { get; set; }
        public List<Violation> Violations { get; set; } 

    }
}
