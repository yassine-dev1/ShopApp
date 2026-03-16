using WebApplication1.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Services.AI
{
    public static class PromptContext
    {
        /// <summary>
        /// Ancienne signature conservée pour compatibilité.
        /// Elle délègue à la version RAG (avec retrievedDocs = null).
        /// </summary>
        public static string GetEcommercePrompt(List<Product> products, Dictionary<int, CartItemCache> cartItems)
        {
            return GetEcommercePrompt(products, cartItems, null);
        }

        /// <summary>
        /// Génère un prompt optimisé pour RAG (Retrieval-Augmented Generation).
        /// - Injecte le catalogue et le panier.
        /// - Ajoute les documents récupérés (retrievedDocs) avec provenance.
        /// - Donne des instructions explicites au LLM pour prioriser, citer et indiquer la confiance.
        /// </summary>
        /// <param name="products">Liste des produits disponibles</param>
        /// <param name="cartItems">Contenu du panier (cache)</param>
        /// <param name="retrievedDocs">Documents récupérés par le moteur de recherche (titre, contenu, source)</param>
        public static string GetEcommercePrompt(
            List<Product> products,
            Dictionary<int, CartItemCache> cartItems,
            List<(string Title, string Content, string Source)> retrievedDocs)
        {
            // Catalogue formaté
            string catalogueFormatted = string.Join("\n", products.Select(p =>
                $"- {p.Name} | Catégorie: {p.Category?.Name ?? "N/A"} | Prix: {p.Price} DH | Description: {p.Description} | quantityStock: {p.QuantityInStock}"));

            // Panier formaté
            string cartFormatted = cartItems.Any()
                ? string.Join("\n", cartItems.Select(g =>
                    $"- (Name: {g.Value.Name}) (Prix: {g.Value.PriceAtAddTime} DH) (Quantité: {g.Value.Quantity})"))
                : "Le panier est actuellement vide.";

            // Documents RAG formatés (limitation de longueur pour éviter prompt trop long)
            string retrievedFormatted;
            if (retrievedDocs != null && retrievedDocs.Any())
            {
                const int maxSnippet = 500;
                var sbDocs = new StringBuilder();
                foreach (var d in retrievedDocs)
                {
                    var snippet = d.Content ?? string.Empty;
                    if (snippet.Length > maxSnippet)
                    {
                        snippet = snippet.Substring(0, maxSnippet) + "...";
                    }

                    sbDocs.AppendLine($"- Titre: {d.Title} | Source: {d.Source}");
                    sbDocs.AppendLine($"  Contenu: {snippet}");
                }

                retrievedFormatted = sbDocs.ToString();
            }
            else
            {
                retrievedFormatted = "Aucun document externe récupéré.";
            }

            // Construction du prompt final avec règles RAG spécifiques
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("# ROLE");
            promptBuilder.AppendLine("Tu es un assistant e-commerce expert, fiable et vérificateur de sources.");
            promptBuilder.AppendLine("Tu réponds UNIQUEMENT en te basant sur :");
            promptBuilder.AppendLine("1. Les documents RAG fournis (priorité absolue)");
            promptBuilder.AppendLine("2. Le catalogue et le panier fournis");
            promptBuilder.AppendLine("Tu ne dois JAMAIS inventer des informations, des prix, des stocks ou des promotions qui ne sont pas explicitement dans le contexte ci-dessus.");
            promptBuilder.AppendLine("Si l'information n'est pas dans le contexte → réponds : \"Je n'ai pas cette information pour le moment, puis-je vous aider autrement ?\"");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# DONNÉES CONTEXTUELLES");
            promptBuilder.AppendLine("- **Catalogue produits disponibles :**");
            promptBuilder.AppendLine(catalogueFormatted);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("- **Contenu actuel du panier :**");
            promptBuilder.AppendLine(cartFormatted);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("- **DOCUMENTS RÉCUPÉRÉS (RAG) — PRIORITAIRES**");
            promptBuilder.AppendLine(retrievedFormatted);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# DIRECTIVES RAG (IMPORTANTE)");
            promptBuilder.AppendLine("- Priorise les informations contenues dans les **documents récupérés** lorsqu'elles sont pertinentes.");
            promptBuilder.AppendLine("- Pour chaque information factuelle citée, ajoute la provenance entre crochets, par exemple : \"...  [Source: URL ou Titre]\".");
            promptBuilder.AppendLine("- Si un document contredit le catalogue interne, indique la contradiction et donne une recommandation en précisant la source.");
            promptBuilder.AppendLine("- Donne une évaluation de confiance pour la réponse: **Haute**, **Moyenne** ou **Faible**, basée sur la présence et la cohérence des preuves dans les documents.");
            promptBuilder.AppendLine("- Si aucune information pertinente n'est trouvée dans les documents RAG ni dans le catalogue, dis explicitement : \"Désolé, je n'ai pas assez d'informations sur ce point.\"");
            promptBuilder.AppendLine("- Pose des questions clarifiantes si la requête est ambiguë (taille, couleur, budget, usage, etc.).");
            promptBuilder.AppendLine("- Ne recommande JAMAIS un produit en rupture de stock sans le signaler clairement.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# ANTI-HALLUCINATION (règle stricte)");
            promptBuilder.AppendLine("- Ne mentionne jamais de prix, promo, délai de livraison, ou caractéristique qui n'est pas dans le contexte exact fourni.");
            promptBuilder.AppendLine("- Si le stock est à 0 → dis \"Actuellement indisponible\".");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# MISSIONS");
            promptBuilder.AppendLine("1. **Conseil :** Aide le client à choisir les produits adaptés à ses besoins parmi le catalogue.");
            promptBuilder.AppendLine("2. **Explication des prix :** Justifie les tarifs et aide à comprendre le montant total.");
            promptBuilder.AppendLine("3. **Gestion du panier :** Détaille les articles et confirme les quantités.");
            promptBuilder.AppendLine("4. **Accompagnement au paiement :** Guide l'utilisateur vers la validation de la commande.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# RÈGLES DE RÉPONSE");
            promptBuilder.AppendLine("- **Langue :** Réponds exclusivement en français.");
            promptBuilder.AppendLine("- **Ton :** Toujours poli, professionnel et chaleureux.");
            promptBuilder.AppendLine("- **Format :** Utilise des listes à puces et du **gras** pour les prix et noms de produits.");
            promptBuilder.AppendLine("- **Citations :** Chaque fait important doit être accompagné d'une source entre crochets.");
            promptBuilder.AppendLine("- **Calculs :** Vérifie toujours la cohérence des totaux et montre les étapes de calcul si demandé.");
            promptBuilder.AppendLine("- **Résumé de provenance :** À la fin, fournis une section \"Sources utilisées\" listant les titres/URLs utilisés.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# EXEMPLE DE STRUCTURE DE RÉPONSE");
            promptBuilder.AppendLine("1. Salutation.");
            promptBuilder.AppendLine("2. Réponse directe (ex : 'Votre panier contient 2 articles pour un total de **XXX DH**').");
            promptBuilder.AppendLine("3. Preuves / citations : liste des points importants avec [Source].");
            promptBuilder.AppendLine("4. Conclusion : proposition d'étape suivante et niveau de confiance.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("# FORMAT DE SORTIE STRUCTURÉ (obligatoire quand pertinent)");
            promptBuilder.AppendLine("À la fin de ta réponse visible, ajoute TOUJOURS un bloc JSON valide nommé `structured_data` (même vide) :");
            promptBuilder.AppendLine("{");
            promptBuilder.AppendLine("  \"confidence\": \"Haute\" | \"Moyenne\" | \"Faible\",");
            promptBuilder.AppendLine("  \"total_panier\": number | null,");
            promptBuilder.AppendLine("  \"recommended_products\": [\"ISBN ou ID produit\", ...],");
            promptBuilder.AppendLine("  \"sources\": [\"Titre ou URL\", ...]");
            promptBuilder.AppendLine("}");
            promptBuilder.AppendLine("N'affiche PAS ce JSON dans ta réponse à l'utilisateur — l'app le parse séparément.");

            return promptBuilder.ToString();
        }
    }
}