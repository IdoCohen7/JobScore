namespace JobScoreServer.DTOs
{
    public record RegisterRequest(string firstName, string lastName, string email, string password, string passwordConfirm);
}
