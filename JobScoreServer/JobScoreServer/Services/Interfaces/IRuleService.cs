using JobScoreServer.DTOs;

namespace JobScoreServer.Services.Interfaces
{
    public interface IRuleService
    {
        Task<List<RuleDTO>> GetRules();
        Task<bool> UpdateRuleWeight(List<UpdateRuleWeightRequest> requests);
    }
}
