namespace JobScoreServer.DTOs
{
  public record ChatMessage(int id, string firstName, string lastName, string content, DateTime createdAt);
}
