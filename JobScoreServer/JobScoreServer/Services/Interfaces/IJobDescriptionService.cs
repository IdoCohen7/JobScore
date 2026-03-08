using JobScoreServer.DTOs;

namespace JobScoreServer.Services.Interfaces
{
    public interface IJobDescriptionService
    {
        Task<JobDescriptionDTO> CreateJobDescription(CreateJobDescriptionDTO request, int userId);
        Task<JobDescriptionEvaluationResultDTO> EvaluateJobDescription(EvaluateJobDescriptionDTO request);
    }
}
