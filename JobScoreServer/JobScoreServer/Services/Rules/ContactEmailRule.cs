using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class ContactEmailRule : IJobEvaluationRule
    {
        private const string RequiredEmail = "hr@company.com";

        public int RuleId => 6;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Check both title and content for the required email
            var combinedText = $"{title ?? string.Empty} {content}";
            
            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return Task.FromResult(false);
            }

            var containsEmail = combinedText.Contains(RequiredEmail, StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(containsEmail);
        }
    }
}
