using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IJobEvaluatorService
    {
        Task EvaluateAndSaveAsync(int jobDescriptionId, string content);
        Task<JobDescriptionEvaluationResultDTO> EvaluateOnlyAsync(string title, string content);
    }
}
