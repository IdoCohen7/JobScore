namespace JobScoreServer.Services.Interfaces
{
    public interface IJobEvaluationRule
    {
        int RuleId { get; }
        Task<bool> EvaluateAsync(string jobDescriptionContent);
        Task<bool> EvaluateAsync(string title, string jobDescriptionContent)
        {
            return EvaluateAsync(jobDescriptionContent);
        }
    }
}
