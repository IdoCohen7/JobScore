using JobScoreServer.DTOs;

namespace JobScoreServer.Services.Interfaces
{
    public interface IJobEvaluatorService
    {
        Task EvaluateAndSaveAsync(int jobDescriptionId, string content);
        Task<JobDescriptionEvaluationResultDTO> EvaluateOnlyAsync(string title, string content);
    }
}
