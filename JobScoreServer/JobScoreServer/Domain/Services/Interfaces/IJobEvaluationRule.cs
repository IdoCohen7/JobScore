namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IJobEvaluationRule
    {
        int RuleId { get; }
        Task<bool> EvaluateAsync(string content, string? title = null);
    }
}
