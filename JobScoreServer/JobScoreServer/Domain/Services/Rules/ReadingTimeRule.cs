using JobScoreServer.Domain.Services.Interfaces;

namespace JobScoreServer.Domain.Services.Rules
{
    public class ReadingTimeRule : IJobEvaluationRule
    {
        private const int AverageWordsPerMinute = 200;
        private const double MinReadingSeconds = 30;
        private const double MaxReadingSeconds = 180;

        public int RuleId => 10;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Combine title and content for total word count
            var combinedText = $"{title ?? string.Empty} {content}";
            
            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return Task.FromResult(false);
            }

            var wordCount = combinedText
                .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var readingTimeSeconds = (wordCount / (double)AverageWordsPerMinute) * 60;
            var isValid = readingTimeSeconds >= MinReadingSeconds
                       && readingTimeSeconds <= MaxReadingSeconds;

            return Task.FromResult(isValid);
        }
    }
}
