namespace WebApplication1.Services.AI.RagService
{
    public interface IOrchestratorRagService
    {
        Task<string> AskAsync(string question, int topkProduct=20, int topkRanker=5);
    }
}
