using JobScoreServer.Data;
using JobScoreServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using JobScoreServer.DTOs;
using Mapster;
using JobScoreServer.Helpers;

namespace JobScoreServer.Services
{
    public class MetricService : IMetricService
    {
        private readonly DBContext _dbcontext;
        public MetricService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<decimal> GetAverageScore()
        {
            try
            {
                var average = await _dbcontext.JobDescriptions.AsNoTracking().AverageAsync(j => j.Score);
                return average;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        public async Task<List<ViolationCount>> GetViolationDistribution()
        {
            try
            {
                // Group by RuleId and count violations
                var distribution = await _dbcontext.Violations
                    .AsNoTracking()
                    .GroupBy(v => v.RuleId)
                    .Select(g => new 
                    { 
                        RuleId = g.Key, 
                        Count = g.Count() 
                    })
                    .ToListAsync();

                // Get all rules
                var rules = await _dbcontext.Rules
                    .AsNoTracking()
                    .ToDictionaryAsync(r => r.Id, r => r.Title);

                // Combine the results
                var result = distribution
                    .Where(d => rules.ContainsKey(d.RuleId))
                    .Select(d => new ViolationCount(
                        rules[d.RuleId],
                        d.Count
                    ))
                    .OrderByDescending(vc => vc.count)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TrendingBuzzword>> GetTopBuzzowrds()
        {
            try
            {
                var topBuzzwords = await _dbcontext.Buzzwords
                    .AsNoTracking()
                    .OrderByDescending(b => b.Count)
                    .Take(3)
                    .ToListAsync();
                    
                return topBuzzwords.Adapt<List<TrendingBuzzword>>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TrendlineDataPoint>> GetWeeklyTrendLine()
        {
            try
            {
                // Get job descriptions from the last 12 weeks
                var twelveWeeksAgo = DateTime.Now.AddDays(-84);
                
                var jobDescriptions = await _dbcontext.JobDescriptions
                    .AsNoTracking()
                    .Where(j => j.CreatedAt >= twelveWeeksAgo)
                    .Select(j => new { j.CreatedAt, j.Score })
                    .ToListAsync();

                // Group by week and calculate average
                var weeklyData = jobDescriptions
                    .GroupBy(j => new
                    {
                        Year = j.CreatedAt.Year,
                        Week = DateTimeHelper.GetWeekOfYear(j.CreatedAt)
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Week,
                        AverageScore = g.Average(x => x.Score),
                        Count = g.Count()
                    })
                    .OrderBy(d => d.Year)
                    .ThenBy(d => d.Week)
                    .Select(d => new TrendlineDataPoint(
                        $"Week {d.Week}, {d.Year}",
                        d.AverageScore,
                        d.Count
                    ))
                    .ToList();

                return weeklyData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TrendlineDataPoint>> GetMonthlyTrendLine()
        {
            try
            {
                // Get job descriptions from the last 12 months
                var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
                
                var jobDescriptions = await _dbcontext.JobDescriptions
                    .AsNoTracking()
                    .Where(j => j.CreatedAt >= twelveMonthsAgo)
                    .Select(j => new { j.CreatedAt, j.Score })
                    .ToListAsync();

                // Group by month and calculate average
                var monthlyData = jobDescriptions
                    .GroupBy(j => new
                    {
                        Year = j.CreatedAt.Year,
                        Month = j.CreatedAt.Month
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        AverageScore = g.Average(x => x.Score),
                        Count = g.Count()
                    })
                    .OrderBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .Select(d => new TrendlineDataPoint(
                        $"{DateTimeHelper.GetMonthName(d.Month)} {d.Year}",
                        d.AverageScore,
                        d.Count
                    ))
                    .ToList();

                return monthlyData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
