using JobScoreServer.Domain.Models;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}
