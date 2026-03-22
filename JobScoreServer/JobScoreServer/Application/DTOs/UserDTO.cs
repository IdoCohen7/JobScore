namespace JobScoreServer.DTOs
{
    public record UserDTO(int id, string firsName, string lastName);

    public record ChatUserDTO(int id, string firstName, string lastName, bool isAdmin);
}
