using JobScoreServer.Models;

namespace JobScoreServer.Services
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}
