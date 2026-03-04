using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Services.Rules
{
    public class ProfessionalismRule : IJobEvaluationRule
    {
        private readonly IBuzzwordService _buzzwordService;

        public ProfessionalismRule(IBuzzwordService buzzwordService)
        {
            _buzzwordService = buzzwordService;
        }

        public int RuleId => 2;

        public Task<bool> EvaluateAsync(string jobDescriptionContent)
        {
            // unused
            return Task.FromResult(true);
        }

        public async Task<bool> EvaluateAsync(string title, string jobDescriptionContent)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(jobDescriptionContent))
            {
                return true;
            }

            var buzzwords = await _buzzwordService.GetAllBuzzowrds();
            
            if (buzzwords == null || !buzzwords.Any())
            {
                return true; // no buzzwords exist
            }

            var combinedText = $"{title} {jobDescriptionContent}".ToLower();
            var foundBuzzwords = new HashSet<int>();

            foreach (var buzzword in buzzwords)
            {
                if (string.IsNullOrWhiteSpace(buzzword.Name))
                {
                    continue;
                }

                // check if buzzwords exists in text
                if (combinedText.Contains(buzzword.Name.ToLower()))
                {
                    foundBuzzwords.Add(buzzword.Id);
                }
            }

            // buzzword counter increment
            foreach (var buzzwordId in foundBuzzwords)
            {
                await _buzzwordService.IncrementBuzzwordCount(buzzwordId);
            }

            // false if no buzzwords were found
            return foundBuzzwords.Count == 0;
        }
    }
}
