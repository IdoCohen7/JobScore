namespace JobScoreServer.Services.Interfaces
{
    public interface IJobEvaluationRule
    {
        int RuleId { get; }
        Task<bool> EvaluateAsync(string jobDescriptionContent);

        /// <summary>
        /// Optional: Evaluate using both title and content
        /// Override this for rules that need title information
        /// </summary>
        Task<bool> EvaluateAsync(string title, string jobDescriptionContent)
        {
            return EvaluateAsync(jobDescriptionContent);
        }
    }
}
