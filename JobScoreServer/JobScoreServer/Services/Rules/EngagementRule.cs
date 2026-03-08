using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class EngagementRule : IJobEvaluationRule
    {
        // Maximum characters allowed in a line before requiring a line break
        private const int MaxCharsPerLine = 75;

        public int RuleId => 5;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Title is not needed for engagement check, only uses content
            if (string.IsNullOrWhiteSpace(content))
            {
                return Task.FromResult(false);
            }

            // Split content into lines
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.None);

            // Check if any line exceeds the maximum character limit
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                // If any line exceeds the limit, fail the rule
                if (trimmedLine.Length > MaxCharsPerLine)
                {
                    return Task.FromResult(false);
                }
            }

            // Pass if all lines are within the character limit
            return Task.FromResult(true);
        }
    }
}
