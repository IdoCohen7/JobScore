using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class ToneOfVoiceRule : IJobEvaluationRule
    {
        // maximum allowed exclamation marks to maintain professional tone
        private const int MaxExclamationMarks = 2;

        public int RuleId => 8;

        public Task<bool> EvaluateAsync(string jobDescriptionContent)
        {
            return Task.FromResult(true);
        }

        public Task<bool> EvaluateAsync(string title, string jobDescriptionContent)
        {
            // combine title and content for comprehensive check
            var combinedText = $"{title} {jobDescriptionContent}";

            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return Task.FromResult(true);
            }

            // count exclamation marks in the combined text
            var exclamationCount = combinedText.Count(c => c == '!');

            // pass if exclamation marks are within acceptable limit
            var isAcceptable = exclamationCount <= MaxExclamationMarks;

            return Task.FromResult(isAcceptable);
        }
    }
}
