using JobScoreServer.Data;
using JobScoreServer.Models;
using JobScoreServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobScoreServer.Services
{
    public class BuzzwordService : IBuzzwordService
    {
        private readonly DBContext _dbcontext;

        public BuzzwordService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Buzzword>> GetAllBuzzowrds()
        {
            try
            {
                var buzzwords = await _dbcontext.Buzzwords.AsNoTracking().ToListAsync();

                if (buzzwords.IsNullOrEmpty())
                {
                    return null;
                }

                return buzzwords;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Buzzword> CreateBuzzword(string name)
        {
            try
            {
                var newBuzzword = new Buzzword
                {
                    Name = name,
                };

                _dbcontext.Buzzwords.Add(newBuzzword);
                await _dbcontext.SaveChangesAsync();
                return newBuzzword;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteBuzzword(int id)
        {
            try
            {
                var buzzword = await _dbcontext.Buzzwords.FirstOrDefaultAsync(b => b.Id == id);

                if (buzzword == null)
                {
                    throw new Exception("Buzzword with that ID not found");
                }

                _dbcontext.Buzzwords.Remove(buzzword);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> IncrementBuzzwordCount(int id)
        {
            try
            {
                var buzzword = await _dbcontext.Buzzwords.FirstOrDefaultAsync(b => b.Id == id);

                if (buzzword == null)
                {
                    throw new Exception("Buzzword with that ID not found");
                }

                buzzword.Count = buzzword.Count + 1;
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
