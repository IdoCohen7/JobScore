namespace JobScoreServer.Application.DTOs
{
    public record JobDescriptionEvaluationResultDTO(decimal score, List<ViolationResultDTO> violations);
    
    public record ViolationResultDTO(int ruleId, string ruleTitle, decimal impact);
}
