using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")] // Protect entire controller with AdminOnly policy
    public class RuleController : ControllerBase
    {
        private readonly IRuleService _ruleService;

        public RuleController(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RuleDTO>>> Get()
        {
            try
            {
                var rules = await _ruleService.GetRules();
                return Ok(rules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("weights")]
        public async Task<IActionResult> UpdateRuleWeight([FromBody] List<UpdateRuleWeightRequest> requests)
        {
            try
            {
                if (requests == null || !requests.Any())
                {
                    return BadRequest(new { error = "Request cannot be empty" });
                }

                var result = await _ruleService.UpdateRuleWeight(requests);

                if (result)
                {
                    return Ok(new { message = "Rule weights updated successfully!" });
                }

                return StatusCode(500, new { error = "Failed to update rule weights" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
