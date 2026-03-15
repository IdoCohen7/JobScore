namespace JobScoreServer.Models
{
    public class ChatroomUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChatroomId { get; set; }
        public Chatroom Chatroom { get; set; }
        public bool IsAdmin { get; set; }

    }
}
