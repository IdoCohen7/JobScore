using JobScoreServer.Domain.Models;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IBuzzwordService
    {
        Task<List<Buzzword>> GetAllBuzzowrds();
        Task<Buzzword> CreateBuzzword(string name);
        Task<bool> DeleteBuzzword(int id);
        Task<bool> IncrementBuzzwordCount(int id);

    }
}
