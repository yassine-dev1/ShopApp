namespace WebApplication1.Services.AI.LLM
{
    public interface ILlmService
    {
        Task<string> GenerateAsync(string prompt);
    }
}

