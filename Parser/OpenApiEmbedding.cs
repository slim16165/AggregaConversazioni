using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;

namespace AggregaConversazioni.Parser;

//https://github.com/OkGoDoIt/OpenAI-API-dotnet
internal class OpenApiEmbedding
{
    public static void Prova()
    {
        OpenAIAPI api = new OpenAIAPI("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"); // shorthand
            
        //Task<EmbeddingResult> CreateEmbeddingAsync(EmbeddingRequest request);

        string manyRows = " Stress → scrivo vari post o a varie ragazze → non ricevo attenzioni e stima → vado in crisi emotiva → ancora più stress e meno chance di calcolare bene le mosse per ottenere ciò che voglio\r\n    **Soluzione:** prendi chi ti vuole! Laura Gallo, Viola Ventura\r\n-   Procrastino? è perché cerco qualcosa da fare che mi piaccia\r\n-   Le liste mi massacrano\r\n-   Sono un lupo solitario → pensa a ross che si confronta con gli altri moderatori del polemista\r\n-   A lavoro non so giustificarmi perchè **non sono mai nel presente** \r\n-   **Sono nel \"vorrei\", \"dovrei\", sarebbe bello se, mi piacerebbe, e se...**\r\n-   **→ non riesco ad affrontare efficacemente le sfide concrete**\r\n-   **Non riesco a vincere una sfida (es. con le donne) → cerco un surrogato**\r\n-     \r\n-   Mi autofreno:\r\n-   chiedo i soldi a Hic? E a mamma?\r\n-   → **rimango a metà** tra faccio e non faccio, dico e non dico\r\n-   Il mio problema nelle relazioni è che mi esprimo da boomer\r\n-   Altro grosso problema è come stimo il mio valore\r\n-   Vorrei essere apprezzato senza cambiare → punto anche troppo sul mostrare i miei difetti senza paura (risultando non adattivo)\r\n-   Quando esco da fb scrivo molto in chat, sono iperattivo, ipomaniacale → stimo il mio valore in base al fatto che riesca a catturare l'attenzione delle ragazze (es. scrivo a una in chat e non mi risponde)\r\n-   Scrivo tanti pvt proprio quando mi sento debole → sui **social** vedo ragazze di 20 anni, truccate, in posa, rifatte, photoshoppate → è normale che mi autosuggestioni → da lì inizio a giustificarmi forse, penso: mio dio questa ragazza è bellissima, è pericolosa\r\n-   Autosuggestione → le donne fortunate → io sfigato **[effetto dato dai social e da scarsa socializzazione]**\r\n-   Ci sono giorni in cui sono depressivo (es. hai avuto news su Stefania? → impotenza, crisi, ecc, ecc)\r\n-   Chiara mi spiegava che c'è tanta competizione, che la femmina deve iniziare presto a fare figli (culturale e biologico)";
            
        string pattern = @"\*\*";
        string[] sentences = Regex.Split(manyRows, pattern);
        var list = NewMethod(sentences, api);

        // for example
        //var result1 = api.Embeddings.CreateEmbeddingAsync("prova");
        // or
        //var result2 = api.Completions.CreateCompletionAsync("A test text for embedding");

            

    }

    private static async IAsyncEnumerable<List<Data>> NewMethod(string[] sentences, OpenAIAPI api)
    {
        foreach (string sentence in sentences)
        {
            EmbeddingResult result1 = await api.Embeddings.CreateEmbeddingAsync(sentence);
            yield return result1.Data;
        }
    }
}