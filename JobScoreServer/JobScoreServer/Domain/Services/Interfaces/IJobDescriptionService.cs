using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IJobDescriptionService
    {
        Task<JobDescriptionDTO> CreateJobDescription(CreateJobDescriptionDTO request, int userId);
        Task<JobDescriptionEvaluationResultDTO> EvaluateJobDescription(EvaluateJobDescriptionDTO request);
    }
}
