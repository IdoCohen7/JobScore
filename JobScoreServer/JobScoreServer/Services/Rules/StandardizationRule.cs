using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class StandardizationRule : IJobEvaluationRule
    {
        private static readonly string[] SeniorityLevels =
        {
            "junior", "jr", "entry", "entry-level",
            "mid", "mid-level", "intermediate",
            "senior", "sr", "lead", "principal", "staff",
            "architect", "expert", "specialist",
            "associate", "manager", "director", "head", "chief",
            "intern", "trainee", "graduate"
        };

        public int RuleId => 7;

        public Task<bool> EvaluateAsync(string jobDescriptionContent)
        {
            
            return Task.FromResult(true);
        }

        public Task<bool> EvaluateAsync(string title, string jobDescriptionContent)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return Task.FromResult(false);
            }

            var lowerTitle = title.ToLower();

            // checks if title contains any seniority level indicator
            var hasSeniorityLevel = SeniorityLevels.Any(level =>
                lowerTitle.Contains(level, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(hasSeniorityLevel);
        }
    }
}
