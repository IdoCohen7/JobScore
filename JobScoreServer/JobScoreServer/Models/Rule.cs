namespace JobScoreServer.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public int Weight { get; set; }
        public List<Violation> Violations { get; set; }

    }
}
