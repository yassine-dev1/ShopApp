using WebApplication1.Services.AI.LLM;
using WebApplication1.Services.AI.Reranking;
using WebApplication1.Services.AI.Retrieval;
using WebApplication1.Services.AI.Prompt;
// NOTE: intentionally NOT 'using WebApplication1.Services.AI' because it contient une classe static `PromptContext`
// we will fully-qualify the instance PromptContext type from the 'Prompt' namespace to avoid ambiguity.

namespace WebApplication1.Services.AI.RagService
{
    public class OrchestratorRagService : IOrchestratorRagService
    {
        private readonly IRetrievalService _retrieval;
        private readonly IRerankerService _reranker;
        private readonly ILlmService _llm;

        public OrchestratorRagService(
            IRetrievalService retrieval,
            IRerankerService reranker,
            ILlmService llm)
        {
            _retrieval = retrieval;
            _reranker = reranker;
            _llm = llm;
        }

        public async Task<string> AskAsync(string question ,  int topkProduct=20 , int topKranker = 5 )
        {
            // 1️⃣ Retrieval
            var docs = await _retrieval.RetrieveAsync(question, topkProduct);
            Console.WriteLine($"documents retreive by search vector   : {docs.Count()}");

            // 2️⃣ Re-ranking
            var ranked = await _reranker.RerankAsync(question, docs, topKranker);
            Console.WriteLine($"documents retreive by  Re-ranker   : {ranked.Count()}");


            Console.WriteLine("****************************** PROMPT GENERATD*************************");
            // 3️⃣ Prompt
            // build prompt via question user and top documents find 
            var prompt =  new WebApplication1.Services.AI.Prompt.PromptContext(question, ranked);

            Console.WriteLine($"prompt : {prompt.BuildPrompt()}");

            // 4️⃣ LLM
            // send request and retreive answer
            var answer = await _llm.GenerateAsync(prompt.BuildPrompt());

            return answer;
        }
    }
}