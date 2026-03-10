using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.Data;
using Microsoft.EntityFrameworkCore;
using JobScoreServer.DTOs;

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

            // get job description to access title
            var jobDescription = await _dbContext.JobDescriptions.FindAsync(jobDescriptionId);
            if (jobDescription == null) return;

            // Validate inputs
            ValidateInputs(content, jobDescription.Title);

            // evaluate each rule
            foreach (var rule in _rules)
            {
                var ruleConfig = rulesFromDb.FirstOrDefault(r => r.Id == rule.RuleId);
                if (ruleConfig == null) continue;

                // pass content and title (as optional parameter)
                var passed = await rule.EvaluateAsync(content, jobDescription.Title);

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

        public async Task<JobDescriptionEvaluationResultDTO> EvaluateOnlyAsync(string title, string content)
        {
            // Validate inputs
            ValidateInputs(content, title);

            var rulesFromDb = await _dbContext.Rules.AsNoTracking().ToListAsync();

            decimal totalScore = 100;
            var violations = new List<ViolationResultDTO>();

            // evaluate each rule
            foreach (var rule in _rules)
            {
                var ruleConfig = rulesFromDb.FirstOrDefault(r => r.Id == rule.RuleId);
                if (ruleConfig == null) continue;

                // pass content and title (as optional parameter)
                var passed = await rule.EvaluateAsync(content, title);

                if (!passed)
                {
                    totalScore -= ruleConfig.Weight;
                    violations.Add(new ViolationResultDTO(
                        ruleId: rule.RuleId,
                        ruleTitle: ruleConfig.Title,
                        impact: ruleConfig.Weight
                    ));
                }
            }

            totalScore = Math.Max(0, totalScore);

            return new JobDescriptionEvaluationResultDTO(totalScore, violations);
        }

        private static void ValidateInputs(string content, string? title)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }
        }
    }
}
