using JobScoreServer.Data;
using JobScoreServer.DTOs;
using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace JobScoreServer.Services
{
    public class ChatService : IChatService
    {
        private readonly DBContext _dbcontext;

        public ChatService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
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

                return await _dbcontext.ChatroomMessages.AsNoTracking().Include(cm => cm.User).ProjectToType<ChatMessage>().FirstOrDefaultAsync(cm => cm.id == newMessage.Id);
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

                return messages.Adapt<List<ChatMessage>>();
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

    }
}
