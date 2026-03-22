using JobScoreServer.Domain.Services.Interfaces;

namespace JobScoreServer.Domain.Services.Rules
{
    public class ToneOfVoiceRule : IJobEvaluationRule
    {
        // Maximum allowed exclamation marks to maintain professional tone
        private const int MaxExclamationMarks = 2;

        public int RuleId => 8;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Combine title and content for comprehensive check
            var combinedText = $"{title ?? string.Empty} {content}";

            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return Task.FromResult(true);
            }

            // Count exclamation marks in the combined text
            var exclamationCount = combinedText.Count(c => c == '!');

            // Pass if exclamation marks are within acceptable limit
            var isAcceptable = exclamationCount <= MaxExclamationMarks;

            return Task.FromResult(isAcceptable);
        }
    }
}
