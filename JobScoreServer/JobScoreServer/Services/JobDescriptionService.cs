using JobScoreServer.Data;
using JobScoreServer.DTOs;
using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Mapster;

namespace JobScoreServer.Services
{
    public class JobDescriptionService : IJobDescriptionService
    {
        private readonly DBContext _dbcontext;
        private readonly IJobEvaluatorService _jobEvaluatorService;

        public JobDescriptionService(DBContext dbcontext, IJobEvaluatorService jobEvaluatorService)
        {
            _dbcontext = dbcontext;
            _jobEvaluatorService = jobEvaluatorService;
        }

        public async Task<JobDescriptionDTO> CreateJobDescription(CreateJobDescriptionDTO request, int userId)
        {
            var description = new JobDescription
            {
                UserId = userId,
                Title = request.title,
                Content = request.content,
                Score = 0
            };

            _dbcontext.JobDescriptions.Add(description);
            await _dbcontext.SaveChangesAsync();

            // trigger evaluation
            await _jobEvaluatorService.EvaluateAndSaveAsync(description.Id, request.content);

            // reload to get updated score
            await _dbcontext.Entry(description).ReloadAsync();

            return description.Adapt<JobDescriptionDTO>();
        }

        public async Task<JobDescriptionEvaluationResultDTO> EvaluateJobDescription(EvaluateJobDescriptionDTO request)
        {
            return await _jobEvaluatorService.EvaluateOnlyAsync(request.title, request.content);
        }
    }
}
