using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class MetricController : ControllerBase
    {
        private readonly IMetricService _service;

        public MetricController(IMetricService service)
        {
            _service = service;            
        }
        // GET: api/<MetricController>
        [HttpGet("average")]
        public async Task<ActionResult<decimal>> GetAverageScore()
        {
            try
            {
                var average = await _service.GetAverageScore();
                return Ok(average);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/metric/violations
        [HttpGet("violations")]
        public async Task<ActionResult<List<ViolationCount>>> GetViolationDistribution()
        {
            try
            {
                var distribution = await _service.GetViolationDistribution();
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/metric/trending-buzzwords
        [HttpGet("trending-buzzwords")]
        public async Task<ActionResult<List<TrendingBuzzword>>> GetTopBuzzwords()
        {
            try
            {
                var topBuzzwords = await _service.GetTopBuzzowrds();
                return Ok(topBuzzwords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/metric/trendline/weekly
        [HttpGet("trendline/weekly")]
        public async Task<ActionResult<List<TrendlineDataPoint>>> GetWeeklyTrendLine()
        {
            try
            {
                var trendline = await _service.GetWeeklyTrendLine();
                return Ok(trendline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/metric/trendline/monthly
        [HttpGet("trendline/monthly")]
        public async Task<ActionResult<List<TrendlineDataPoint>>> GetMonthlyTrendLine()
        {
            try
            {
                var trendline = await _service.GetMonthlyTrendLine();
                return Ok(trendline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
