using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class JobDescriptionController : ControllerBase
    {
        private readonly IJobDescriptionService _jobDescriptionService;
        public JobDescriptionController(IJobDescriptionService jobDescriptionService)
        {
            _jobDescriptionService = jobDescriptionService;
        }

        // POST api/<JobDescriptionController>
        [HttpPost]
        public async Task<ActionResult<JobDescriptionDTO>> Post([FromBody] CreateJobDescriptionDTO request)
        {
            try
            {
                // extract user ID from JWT 
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (userIdClaim == null)
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { error = "Invalid user ID in token" });
                }

                var description = await _jobDescriptionService.CreateJobDescription(request, userId);

                return Ok(description);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST api/<JobDescriptionController>/evaluate
        [HttpPost("evaluate")]
        public async Task<ActionResult<JobDescriptionEvaluationResultDTO>> Evaluate([FromBody] EvaluateJobDescriptionDTO request)
        {
            try
            {
                var result = await _jobDescriptionService.EvaluateJobDescription(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
