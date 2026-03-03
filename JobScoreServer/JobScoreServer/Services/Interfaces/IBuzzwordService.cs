using JobScoreServer.Models;

namespace JobScoreServer.Services.Interfaces
{
    public interface IBuzzwordService
    {
        Task<List<Buzzword>> GetAllBuzzowrds();
        Task<Buzzword> CreateBuzzword(string name);
        Task<bool> DeleteBuzzword(int id);
        Task<bool> IncrementBuzzwordCount(int id);

    }
}
