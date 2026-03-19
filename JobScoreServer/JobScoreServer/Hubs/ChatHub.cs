using Microsoft.AspNetCore.SignalR;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace JobScoreServer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinRoom(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        }

        public async Task LeaveRoom(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task SendMessage(int chatId, string content)
        {
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new HubException("User not found");
            }

            var chatMessage = await _chatService.SendMessage(userId, chatId, content);

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatMessage);

            try
            {
                var firstNonEmpty = content?.Replace("\r\n", "\n").Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();
                if (!string.IsNullOrEmpty(firstNonEmpty) && firstNonEmpty.Equals("@BOT", StringComparison.OrdinalIgnoreCase))
                {
                    var botMessage = await _chatService.GenerateBotMessageAsync(chatId, content);
                    if (botMessage != null)
                    {
                        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", botMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}