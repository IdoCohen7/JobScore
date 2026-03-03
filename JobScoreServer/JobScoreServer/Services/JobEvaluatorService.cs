using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.Data;
using Microsoft.EntityFrameworkCore;

namespace JobScoreServer.Services
{
    public class JobEvaluatorService : IJobEvaluatorService
    {
        private readonly DBContext _dbContext;
        private readonly IEnumerable<IJobEvaluationRule> _rules;

        public JobEvaluatorService(DBContext dbContext, IEnumerable<IJobEvaluationRule> rules)
        {
            _dbContext = dbContext;
            _rules = rules;
        }

        public async Task EvaluateAndSaveAsync(int jobDescriptionId, string content)
        {
            var rulesFromDb = await _dbContext.Rules.AsNoTracking().ToListAsync();

            decimal totalScore = 100;
            var violations = new List<Violation>();

            // Get job description to access title
            var jobDescription = await _dbContext.JobDescriptions.FindAsync(jobDescriptionId);
            if (jobDescription == null) return;

            // Evaluate each rule
            foreach (var rule in _rules)
            {
                var ruleConfig = rulesFromDb.FirstOrDefault(r => r.Id == rule.RuleId);
                if (ruleConfig == null) continue;

                // Pass both title and content
                var passed = await rule.EvaluateAsync(jobDescription.Title, content);

                if (!passed)
                {
                    totalScore -= ruleConfig.Weight;
                    violations.Add(new Violation
                    {
                        JobDescriptionId = jobDescriptionId,
                        RuleId = rule.RuleId,
                        Impact = ruleConfig.Weight
                    });
                }
            }

            totalScore = Math.Max(0, totalScore);

            jobDescription.Score = totalScore;

            if (violations.Any())
            {
                _dbContext.Violations.AddRange(violations);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
