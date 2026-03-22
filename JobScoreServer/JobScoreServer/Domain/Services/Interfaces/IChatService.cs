using JobScoreServer.DTOs;
using JobScoreServer.Models;

namespace JobScoreServer.Services.Interfaces
{
    public interface IChatService
    {
        Task<int> CreateChatroom(string title, int userId);
        Task<ChatMessage> SendMessage(int userId, int chatId, string content);
        Task<List<ChatMessage>> GetChatMessages(int id);
        Task<List<UserDTO>> GetOuterUsers(int id);
        Task<List<ChatUserDTO>> GetInnerUsers(int id);
        Task<bool> AddUserToChatroom(int actingUserId, int targetUserId, int chatroomId);
        Task<bool> RemoveUserFromChatroom(int actingUserId, int targetUserId, int chatroomId);
        Task<bool> SetUserAsAdmin(int actingUserId, int targetUserId, int chatroomId);
        Task<List<Chatroom>> GetChatrooms(int userId);
        Task<bool> DeleteChatroom(int actingUserId, int chatroomId);
        Task<object?> GenerateBotMessageAsync(int chatId, string content);

    }
}
