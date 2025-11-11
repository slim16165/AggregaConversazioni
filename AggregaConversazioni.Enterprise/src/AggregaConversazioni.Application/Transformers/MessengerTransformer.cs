using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public class MessengerTransformer : RegexBasedTransformer
{
    public MessengerTransformer()
    {
        // Rimuovi "You unsent a message"
        RegexReplacements.Add((@"You unsent a message", ""));

        // Sostituisci nomi speaker con "Lei: " o "Io: "
        // Nota: In produzione questi pattern dovrebbero essere configurabili
        RegexReplacements.Add((@"You sent", "Io: "));
        RegexReplacements.Add((@"^Enter", ""));

        // Rimuovi timestamp
        RegexReplacements.Add((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""));

        // Gestisci reply
        RegexReplacements.Add((@"(.+?) replied to you", "Io: "));
        RegexReplacements.Add((@"You replied to (.+?)", "Io: "));
        RegexReplacements.Add((@"(.+?) replied to themself", "Lei: "));
    }
}
