using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class ReadingTimeRule : IJobEvaluationRule
    {
        private const int AverageWordsPerMinute = 200;
        private const double MinReadingSeconds = 30;
        private const double MaxReadingSeconds = 180;

        public int RuleId => 10;

        public Task<bool> EvaluateAsync(string jobDescriptionContent)
        {
            if (string.IsNullOrWhiteSpace(jobDescriptionContent))
            {
                return Task.FromResult(false);
            }

            var wordCount = jobDescriptionContent
                .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var readingTimeSeconds = (wordCount / (double)AverageWordsPerMinute) * 60;
            var isValid = readingTimeSeconds >= MinReadingSeconds
                       && readingTimeSeconds <= MaxReadingSeconds;

            return Task.FromResult(isValid);
        }

        // Uses default implementation that ignores title
    }
}
