namespace JobScoreServer.Models
{
    public class Chatroom
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt {  get; set; } = DateTime.Now;
        public List<ChatroomUser> ChatroomUsers { get; set; } = [];

    }
}
