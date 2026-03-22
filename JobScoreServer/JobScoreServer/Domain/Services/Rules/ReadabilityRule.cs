using JobScoreServer.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace JobScoreServer.Domain.Services.Rules
{
    public class ReadabilityRule : IJobEvaluationRule
    {
        // Minimum required bullet points or structural elements for readability
        private const int MinStructuralElements = 3;

        // Common bullet point indicators and list markers
        private static readonly string[] BulletIndicators = 
        {
            "•", "●", "○", "◦", "■", "□", "▪", "▫", "–", "—", "⋅", "*", "+"
        };

        public int RuleId => 1;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Title is not needed for readability check, only uses content
            if (string.IsNullOrWhiteSpace(content))
            {
                return Task.FromResult(false);
            }

            var structuralElementCount = 0;

            // Count bullet points and special list markers
            foreach (var indicator in BulletIndicators)
            {
                structuralElementCount += content.Count(c => c.ToString() == indicator);
            }

            // Count lines starting with numbers (numbered lists: 1., 2., 3., etc.)
            var numberedListPattern = @"^\s*\d+[\.\)]\s+";
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            structuralElementCount += lines.Count(line => Regex.IsMatch(line, numberedListPattern));

            // Count lines starting with dashes or asterisks followed by space (markdown-style bullets)
            var markdownBulletPattern = @"^\s*[-*]\s+";
            structuralElementCount += lines.Count(line => Regex.IsMatch(line, markdownBulletPattern) 
                && !Regex.IsMatch(line, numberedListPattern)); // Avoid double counting

            // Pass if we have at least the minimum required structural elements
            return Task.FromResult(structuralElementCount >= MinStructuralElements);
        }
    }
}
