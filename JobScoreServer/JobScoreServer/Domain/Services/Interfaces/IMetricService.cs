using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IMetricService
    {
        Task<decimal> GetAverageScore();

        Task<List<ViolationCount>> GetViolationDistribution();

        Task<List<TrendingBuzzword>> GetTopBuzzowrds();

        Task<List<TrendlineDataPoint>> GetWeeklyTrendLine();

        Task<List<TrendlineDataPoint>> GetMonthlyTrendLine();
    }
}
