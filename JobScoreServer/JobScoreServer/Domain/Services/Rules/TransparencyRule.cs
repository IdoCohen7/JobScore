using JobScoreServer.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace JobScoreServer.Domain.Services.Rules
{
    public class TransparencyRule : IJobEvaluationRule
    {
        // lazy initialization to avoid type initializer issues
        // matches patterns like: $50000, 50000$, $50k, 50k$, $50,000, 50000-60000$, $40k-50k, etc.
        private static readonly Lazy<Regex> _salaryPattern = new Lazy<Regex>(() =>
            new Regex(@"\$\s*\d+[\d,]*\.?\d*[kK]?|\d+[\d,]*\.?\d*[kK]?\s*\$|\d+[\d,]*\.?\d*[kK]?\s*[-–—]\s*\d+[\d,]*\.?\d*[kK]?\s*\$|\$\s*\d+[\d,]*\.?\d*[kK]?\s*[-–—]\s*\d+[\d,]*\.?\d*[kK]?",
                RegexOptions.Compiled));

        public int RuleId => 4;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // title is not needed for salary transparency check, only uses content
            if (string.IsNullOrWhiteSpace(content))
            {
                return Task.FromResult(false);
            }

            // check if content contains any salary information (number or range with $)
            var hasSalaryInfo = _salaryPattern.Value.IsMatch(content);

            return Task.FromResult(hasSalaryInfo);
        }
    }
}
