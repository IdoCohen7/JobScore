namespace JobScoreServer.Services.Interfaces
{
    public interface IJobEvaluatorService
    {
        Task EvaluateAndSaveAsync(int jobDescriptionId, string content);
    }
}
