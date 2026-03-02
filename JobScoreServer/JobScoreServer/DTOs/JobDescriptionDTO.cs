namespace JobScoreServer.DTOs
{
    public record JobDescriptionDTO(int id, int userId, string title, string content, DateTime createdAt, decimal score);
}
