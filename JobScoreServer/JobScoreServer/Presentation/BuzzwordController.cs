using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")] 
    public class BuzzwordController : ControllerBase
    {
        private readonly IBuzzwordService _buzzwordService;

        public BuzzwordController(IBuzzwordService buzzwordService)
        {
            _buzzwordService = buzzwordService;
        }

        // GET: api/buzzword
        [HttpGet]
        public async Task<ActionResult<List<Buzzword>>> GetAll()
        {
            try
            {
                var buzzwords = await _buzzwordService.GetAllBuzzowrds();

                if (buzzwords == null || !buzzwords.Any())
                {
                    return Ok(new List<Buzzword>()); 
                }

                return Ok(buzzwords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/buzzword
        [HttpPost]
        public async Task<ActionResult<Buzzword>> Create([FromBody] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { error = "Buzzword name is required" });
                }

                var buzzword = await _buzzwordService.CreateBuzzword(name.Trim());
                return CreatedAtAction(nameof(GetAll), new { id = buzzword.Id }, buzzword);
            }
            catch (DbUpdateException)
            {
                return Conflict(new { error = "A buzzword with this name already exists" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: api/buzzword/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _buzzwordService.DeleteBuzzword(id);

                if (result)
                {
                    return Ok(new { message = "Buzzword deleted successfully" });
                }

                return StatusCode(500, new { error = "Failed to delete buzzword" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
