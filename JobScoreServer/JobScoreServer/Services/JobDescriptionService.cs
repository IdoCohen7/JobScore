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

        public JobDescriptionService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<JobDescriptionDTO> CreateJobDescription(CreateJobDescriptionDTO request, int userId)
        {
            try
            {
                var description = new JobDescription
                {
                    UserId = userId, // received from controller
                    Title = request.title,
                    Content = request.content,
                    Score = 0
                };

                _dbcontext.JobDescriptions.Add(description);
                await _dbcontext.SaveChangesAsync();

                return description.Adapt<JobDescriptionDTO>();
            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
