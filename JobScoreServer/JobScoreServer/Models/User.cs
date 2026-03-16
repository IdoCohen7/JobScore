namespace JobScoreServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash  { get; set; }
        public bool IsAdmin { get; set; } = false;
        public List<JobDescription> JobDescriptions { get; set; }
        public List<ChatroomUser> ChatroomUsers { get; set; }
        public List<ChatroomMessage> ChatroomMessages { get; set; } 

    }
}
