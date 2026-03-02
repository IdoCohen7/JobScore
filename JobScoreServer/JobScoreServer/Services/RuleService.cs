using JobScoreServer.Data;
using JobScoreServer.DTOs;
using JobScoreServer.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JobScoreServer.Services
{
    public class RuleService : IRuleService
    {
        private readonly DBContext _dbcontext;

        public RuleService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<RuleDTO>> GetRules()
        {
            try
            {
                var rules = await _dbcontext.Rules.AsNoTracking().ToListAsync();
                return rules.Adapt<List<RuleDTO>>();
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateRuleWeight(List<UpdateRuleWeightRequest> requests)
        {

            if (requests.Sum(r => r.weight) != 100)
            {
                throw new Exception("Sum of weights must be equal to 100!");
            }

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var rules = await _dbcontext.Rules.ToListAsync();

                foreach (var request in requests)
                {
                    var rule = rules.FirstOrDefault(r => r.Id == request.id);

                    if (rule != null)
                    {
                        rule.Weight = request.weight;
                    }
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}

