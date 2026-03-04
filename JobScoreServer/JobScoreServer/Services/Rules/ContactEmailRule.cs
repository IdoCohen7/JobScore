using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class ContactEmailRule : IJobEvaluationRule
    {
        private const string RequiredEmail = "hr@company.com";

        public int RuleId => 9;

        public Task<bool> EvaluateAsync(string jobDescriptionContent)
        {
            if (string.IsNullOrWhiteSpace(jobDescriptionContent))
            {
                return Task.FromResult(false);
            }

            var containsEmail = jobDescriptionContent.Contains(RequiredEmail, StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(containsEmail);
        }

        
    }
}
