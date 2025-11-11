using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public class InstagramTransformer : RegexBasedTransformer
{
    public InstagramTransformer()
    {
        // Rimuovi giorni della settimana e orari
        RegexReplacements.Add((@"(Lunedì|Martedì|Mercoledì|Giovedì|Sabato|Domenica) \d{1,2}:\d{2}", ""));

        // Converti "Immagine del profilo di" in "Lei: "
        RegexReplacements.Add((@"^Immagine del profilo di (.+?)$", "Lei: "));

        // Rimuovi timestamp
        RegexReplacements.Add((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""));

        // Rimuovi elementi UI
        RegexReplacements.Add((@"Annulla l'invio del messaggio", ""));
        RegexReplacements.Add((@"Mi piace", ""));
        RegexReplacements.Add((@"Copia", ""));
        RegexReplacements.Add((@"❤️", ""));
    }
}
