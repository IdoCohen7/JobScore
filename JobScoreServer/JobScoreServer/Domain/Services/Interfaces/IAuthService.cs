using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Login(string email, string password);
        Task<string> Register(RegisterRequest request);
    }
}
