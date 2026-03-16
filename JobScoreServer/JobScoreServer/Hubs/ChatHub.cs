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

        // כשמשתמש נכנס לחדר ב-React, הוא קורא למתודה הזו
        public async Task JoinRoom(int chatId)
        {
            // הוספת החיבור הנוכחי לקבוצה שנקראת על שם ה-ID של החדר
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        }

        // כשמשתמש עוזב את החדר או סוגר את הטאב
        public async Task LeaveRoom(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        // שליחת הודעה בזמן אמת
        public async Task SendMessage(int chatId, string content)
        {
            // שליפת ה-UserId מה-Claims של המשתמש המחובר
            // וודא שבהגדרות ה-JWT שלך ה-NameIdentifier מוגדר
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new HubException("User not found");
            }

            // 1. שמירה ב-DB דרך ה-Service שכתבת
            var chatMessage = await _chatService.SendMessage(userId, chatId, content);

            // 2. הפצת ההודעה לכל מי שנמצא בקבוצה של החדר הזה (כולל השולח)
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatMessage);
        }
    }
}