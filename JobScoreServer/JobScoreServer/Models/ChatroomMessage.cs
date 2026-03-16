namespace JobScoreServer.Models
{
    public class ChatroomMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User User { get; set; }
        public int UserId { get; set; }
        public Chatroom Chatroom { get; set; }
        public int ChatroomId { get; set; }
    }
}
