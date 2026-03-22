using JobScoreServer.DTOs;
using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobScoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return null;

            if (!int.TryParse(userIdClaim, out var userId))
                return null;

            return userId;
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest(new { error = "Title is required" });

                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var chatId = await _chatService.CreateChatroom(title.Trim(), userId.Value);
                return CreatedAtAction(nameof(GetChatrooms), new { id = chatId }, new { id = chatId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{chatId:int}/messages")]
        public async Task<IActionResult> SendMessage(int chatId, [FromBody] string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return BadRequest(new { error = "Content is required" });

                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var message = await _chatService.SendMessage(userId.Value, chatId, content.Trim());
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{chatId:int}/messages")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            try
            {
                var messages = await _chatService.GetChatMessages(chatId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{chatId:int}/outer-users")]
        public async Task<IActionResult> GetOuterUsers(int chatId)
        {
            try
            {
                var users = await _chatService.GetOuterUsers(chatId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{chatId:int}/inner-users")]
        public async Task<IActionResult> GetInnerUsers(int chatId)
        {
            try
            {
                var users = await _chatService.GetInnerUsers(chatId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{chatId:int}/users/{targetUserId:int}")]
        public async Task<IActionResult> AddUser(int chatId, int targetUserId)
        {
            try
            {
                var actingUserId = GetUserIdFromClaims();
                if (actingUserId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var result = await _chatService.AddUserToChatroom(actingUserId.Value, targetUserId, chatId);
                if (result) return Ok(new { message = "User added to chatroom" });
                return StatusCode(500, new { error = "Failed to add user to chatroom" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{chatId:int}/users/{targetUserId:int}")]
        public async Task<IActionResult> RemoveUser(int chatId, int targetUserId)
        {
            try
            {
                var actingUserId = GetUserIdFromClaims();
                if (actingUserId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var result = await _chatService.RemoveUserFromChatroom(actingUserId.Value, targetUserId, chatId);
                if (result) return Ok(new { message = "User removed from chatroom" });
                return StatusCode(500, new { error = "Failed to remove user from chatroom" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{chatId:int}/admins/{targetUserId:int}")]
        public async Task<IActionResult> SetAdmin(int chatId, int targetUserId)
        {
            try
            {
                var actingUserId = GetUserIdFromClaims();
                if (actingUserId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var result = await _chatService.SetUserAsAdmin(actingUserId.Value, targetUserId, chatId);
                if (result) return Ok(new { message = "User promoted to admin" });
                return StatusCode(500, new { error = "Failed to set user as admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetChatrooms()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var chatrooms = await _chatService.GetChatrooms(userId.Value);
                return Ok(chatrooms);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{chatId:int}")]
        public async Task<IActionResult> DeleteChatroom(int chatId)
        {
            try
            {
                var actingUserId = GetUserIdFromClaims();
                if (actingUserId == null)
                    return Unauthorized(new { error = "User not authenticated" });

                var result = await _chatService.DeleteChatroom(actingUserId.Value, chatId);
                if (result) return Ok(new { message = "Chatroom deleted" });
                return StatusCode(500, new { error = "Failed to delete chatroom" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
