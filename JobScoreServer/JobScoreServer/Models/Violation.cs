namespace JobScoreServer.Models
{
    public class Violation
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int JobDescriptionId { get; set; }
        public decimal Impact { get; set; }
        public JobDescription JobDescription { get; set; }
        public Rule Rule { get; set; }
    }
}
