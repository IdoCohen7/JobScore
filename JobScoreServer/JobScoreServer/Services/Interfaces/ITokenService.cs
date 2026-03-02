using JobScoreServer.Models;

namespace JobScoreServer.Services.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}
