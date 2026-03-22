using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Domain.Services.Interfaces
{
    public interface IRuleService
    {
        Task<List<RuleDTO>> GetRules();
        Task<bool> UpdateRuleWeight(List<UpdateRuleWeightRequest> requests);
    }
}
