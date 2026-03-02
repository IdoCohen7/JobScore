using JobScoreServer.DTOs;

namespace JobScoreServer.Services
{
    public interface IAuthService
    {
        Task<string> Login(string email, string password);
        Task<string> Register(RegisterRequest request);
    }
}
