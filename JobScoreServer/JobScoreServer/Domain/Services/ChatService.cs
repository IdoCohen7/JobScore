using JobScoreServer.Data;
using JobScoreServer.DTOs;
using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Linq;

namespace JobScoreServer.Services
{
    public class ChatService : IChatService
    {
        private readonly DBContext _dbcontext;
        private readonly IJobEvaluatorService _jobEvaluationService;

        public ChatService(DBContext dbcontext, IJobEvaluatorService jobEvaluationService)
        {
            _dbcontext = dbcontext;
            _jobEvaluationService = jobEvaluationService;
        }

        public async Task<int> CreateChatroom(string title, int userId)
        {
            try
            {
                var chatroom = new Chatroom
                {
                    Title = title,
                    ChatroomUsers = new List<ChatroomUser>
                    {
                        new ChatroomUser {
                            UserId = userId,
                            IsAdmin = true,
                        }

                    }
                };

                _dbcontext.Chatrooms.Add(chatroom);
                await _dbcontext.SaveChangesAsync();

                return chatroom.Id;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ChatMessage> SendMessage(int userId, int chatId, string content)
        {
            try
            {
                var newMessage = new ChatroomMessage
                {
                    UserId = userId,
                    ChatroomId = chatId,
                    Content = content
                };

                _dbcontext.ChatroomMessages.Add(newMessage);
                await _dbcontext.SaveChangesAsync();

                // Load the saved entity including the user, then map to DTO on the client side
                var messageEntity = await _dbcontext.ChatroomMessages
                    .AsNoTracking()
                    .Include(cm => cm.User)
                    .FirstOrDefaultAsync(cm => cm.Id == newMessage.Id);

                if (messageEntity == null)
                    return null;

                // Manually construct DTO to ensure user names are included
                return new ChatMessage(
                    messageEntity.Id,
                    messageEntity.User?.FirstName,
                    messageEntity.User?.LastName,
                    messageEntity.Content,
                    messageEntity.CreatedAt
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ChatMessage>> GetChatMessages(int id)
        {
            try
            {
                var messages = await _dbcontext.ChatroomMessages.AsNoTracking().Where(cm => cm.ChatroomId == id)
                    .Include(cm => cm.User)
                    .OrderBy(cm => cm.CreatedAt)
                    .ToListAsync();

                return messages.Select(m => new ChatMessage(m.Id, m.User?.FirstName, m.User?.LastName, m.Content, m.CreatedAt)).ToList();
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserDTO>> GetOuterUsers(int id)
        {
            // Users who are NOT participating in the given chatroom
            try
            {
                var users = await _dbcontext.Users
                    .AsNoTracking()
                    .Where(u => !u.ChatroomUsers.Any(cu => cu.ChatroomId == id))
                    .Select(u => new UserDTO(u.Id, u.FirstName, u.LastName))
                    .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ChatUserDTO>> GetInnerUsers(int id)
        {
            // Users who ARE members of the given chatroom
            try
            {
                var users = await _dbcontext.ChatroomUsers
                    .AsNoTracking()
                    .Where(cu => cu.ChatroomId == id)
                    .Include(cu => cu.User)
                    .Select(cu => new ChatUserDTO(cu.User.Id, cu.User.FirstName, cu.User.LastName, cu.IsAdmin))
                    .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> AddUserToChatroom(int actingUserId, int targetUserId, int chatroomId)
        {
            try
            {
                var acting = await _dbcontext.ChatroomUsers.AsNoTracking().FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == actingUserId);
                if (acting == null || !acting.IsAdmin)
                    throw new UnauthorizedAccessException("Acting user is not an admin in the chatroom.");

                var existing = await _dbcontext.ChatroomUsers.FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == targetUserId);
                if (existing != null)
                    return true; // already a member

                var toAdd = new ChatroomUser { ChatroomId = chatroomId, UserId = targetUserId, IsAdmin = false };
                _dbcontext.ChatroomUsers.Add(toAdd);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveUserFromChatroom(int actingUserId, int targetUserId, int chatroomId)
        {
            try
            {
                var acting = await _dbcontext.ChatroomUsers.AsNoTracking().FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == actingUserId);
                if (acting == null || !acting.IsAdmin)
                    throw new UnauthorizedAccessException("Acting user is not an admin in the chatroom.");

                var existing = await _dbcontext.ChatroomUsers.FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == targetUserId);
                if (existing == null)
                    return true; // already not a member

                _dbcontext.ChatroomUsers.Remove(existing);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SetUserAsAdmin(int actingUserId, int targetUserId, int chatroomId)
        {
            try
            {
                var acting = await _dbcontext.ChatroomUsers.AsNoTracking().FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == actingUserId);
                if (acting == null || !acting.IsAdmin)
                    throw new UnauthorizedAccessException("Acting user is not an admin in the chatroom.");

                var existing = await _dbcontext.ChatroomUsers.FirstOrDefaultAsync(cu => cu.ChatroomId == chatroomId && cu.UserId == targetUserId);
                if (existing == null)
                {
                    // add as admin
                    var toAdd = new ChatroomUser { ChatroomId = chatroomId, UserId = targetUserId, IsAdmin = true };
                    _dbcontext.ChatroomUsers.Add(toAdd);
                }
                else
                {
                    existing.IsAdmin = true;
                    _dbcontext.ChatroomUsers.Update(existing);
                }

                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Returns all chatrooms the given user is a member of
        public async Task<List<Chatroom>> GetChatrooms(int userId)
        {
            try
            {
                var chatrooms = await _dbcontext.Chatrooms
                    .AsNoTracking()
                    .Where(c => c.ChatroomUsers.Any(cu => cu.UserId == userId))
                    .ToListAsync();

                return chatrooms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Deletes a chatroom if the acting user is a member and an admin
        public async Task<bool> DeleteChatroom(int actingUserId, int chatroomId)
        {
            try
            {
                var chatroom = await _dbcontext.Chatrooms
                    .Include(c => c.ChatroomUsers)
                    .FirstOrDefaultAsync(c => c.Id == chatroomId);

                if (chatroom == null)
                    return true; // nothing to delete

                var acting = chatroom.ChatroomUsers.FirstOrDefault(cu => cu.UserId == actingUserId);
                if (acting == null || !acting.IsAdmin)
                    throw new UnauthorizedAccessException("Acting user is not an admin in the chatroom.");

                _dbcontext.Chatrooms.Remove(chatroom);
                await _dbcontext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object?> GenerateBotMessageAsync(int chatId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content)) return null;

                // checking input
                var lines = content.Replace("\r\n", "\n").Split('\n');
                var firstNonEmptyIndex = Array.FindIndex(lines, l => !string.IsNullOrWhiteSpace(l));
                if (firstNonEmptyIndex < 0) return null;
                var firstLine = lines[firstNonEmptyIndex].Trim();
                if (!firstLine.Equals("@BOT", StringComparison.OrdinalIgnoreCase)) return null;
                var titleIndex = Array.FindIndex(lines, firstNonEmptyIndex + 1, l => !string.IsNullOrWhiteSpace(l));
                if (titleIndex < 0) return null;
                var title = lines[titleIndex].Trim();
                var contentLines = lines.Skip(titleIndex + 1).ToArray();
                var body = string.Join("\n", contentLines).Trim();
                if (string.IsNullOrWhiteSpace(body)) return null;

                // evaluating
                var evalResult = await _jobEvaluationService.EvaluateOnlyAsync(title, body);

                // building string response for chat
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Job Description Evaluation — Title: {title}");
                sb.AppendLine($"Score: {evalResult.score}");
                if (evalResult.violations != null && evalResult.violations.Any())
                {
                    sb.AppendLine("Violations:");
                    foreach (var v in evalResult.violations)
                    {
                        sb.AppendLine($"- {v.ruleTitle} (Impact: {v.impact})");
                    }
                }
                else
                {
                    sb.AppendLine("No violations. Good job!");
                }

                var botContent = sb.ToString().TrimEnd();

                var botEmail = "bot@system.local";
                var botUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == botEmail);
                

                // creating the bot message
                var botMessageEntity = new ChatroomMessage
                {
                    UserId = botUser.Id,
                    ChatroomId = chatId,
                    Content = botContent
                };

                _dbcontext.ChatroomMessages.Add(botMessageEntity);
                await _dbcontext.SaveChangesAsync();

                var saved = await _dbcontext.ChatroomMessages
                    .AsNoTracking()
                    .Include(cm => cm.User)
                    .FirstOrDefaultAsync(cm => cm.Id == botMessageEntity.Id);

                if (saved == null) return null;

                return new ChatMessage(
                    saved.Id,
                    saved.User?.FirstName,
                    saved.User?.LastName,
                    saved.Content,
                    saved.CreatedAt
                );
            }
            catch
            {
                return null;
            }
        }

    }
}
