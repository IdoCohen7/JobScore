using JobScoreServer.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace JobScoreServer.Domain.Services.Rules
{
    public class ProfessionalismRule : IJobEvaluationRule
    {
        private readonly IBuzzwordService _buzzwordService;

        public ProfessionalismRule(IBuzzwordService buzzwordService)
        {
            _buzzwordService = buzzwordService;
        }

        public int RuleId => 2;

        public async Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Combine title and content for searching
            var combinedText = $"{title ?? string.Empty} {content}";
            
            // If both are empty, pass the rule
            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return true;
            }

            // Get all buzzwords from the database
            var buzzwords = await _buzzwordService.GetAllBuzzowrds();
            
            // If no buzzwords exist in database, pass the rule
            if (buzzwords == null || !buzzwords.Any())
            {
                return true;
            }

            var foundBuzzwordIds = new HashSet<int>();

            foreach (var buzzword in buzzwords)
            {
                if (string.IsNullOrWhiteSpace(buzzword.Name))
                {
                    continue;
                }

                // Use regex with word boundaries to match whole words only
                // \b ensures we match "rockstar" but not "rock" in "rockstar"
                var pattern = $@"\b{Regex.Escape(buzzword.Name)}\b";
                
                if (Regex.IsMatch(combinedText, pattern, RegexOptions.IgnoreCase))
                {
                    foundBuzzwordIds.Add(buzzword.Id);
                }
            }

            // Increment count for each found buzzword
            foreach (var buzzwordId in foundBuzzwordIds)
            {
                await _buzzwordService.IncrementBuzzwordCount(buzzwordId);
            }

            // Return true (pass) if NO buzzwords found, false (fail) if ANY buzzwords found
            return foundBuzzwordIds.Count == 0;
        }
    }
}
