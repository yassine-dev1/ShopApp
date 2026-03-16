using System.Text;
using WebApplication1.Services.AI.Retrieval;

namespace WebApplication1.Services.AI.Prompt
{
    public class PromptContext
    {
        public string UserQuestion { get; }

        public List<RetrievedDocument> Documents { get; }

        public int MaxDocuments { get; set; } = 5;

        public PromptContext(string question, List<RetrievedDocument> documents)
        {
            UserQuestion = question;
            Documents = documents;
        }

        public string BuildPrompt()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Tu es un assistant intelligent spécialisé dans un site e-commerce.");
            sb.AppendLine("Ta mission est d'aider les utilisateurs à trouver les produits les plus pertinents.");
            sb.AppendLine();
            sb.AppendLine("Instructions :");
            sb.AppendLine("- Utilise uniquement les informations des documents fournis.");
            sb.AppendLine("- Si l'information n'existe pas dans les documents, dis clairement que tu ne sais pas.");
            sb.AppendLine("- Donne une réponse claire et utile.");
            sb.AppendLine("- Si plusieurs produits correspondent, propose les meilleurs.");
            sb.AppendLine("- Mentionne les caractéristiques importantes (prix, catégorie, description).");
            sb.AppendLine();

            sb.AppendLine("DOCUMENTS DISPONIBLES :");
            sb.AppendLine("--------------------------------------------------");

            int count = 0;

            foreach (var doc in Documents)
            {
                if (count >= MaxDocuments)
                    break;

                sb.AppendLine($"Document {count + 1}:");
                sb.AppendLine(doc.Content);
                sb.AppendLine();

                Console.WriteLine($"Document {count + 1}: {doc.Content}");

                count++;
            }

            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine($"QUESTION UTILISATEUR : {UserQuestion}");
            sb.AppendLine();
            sb.AppendLine("Réponse :");

            return sb.ToString();
        }
    }
}