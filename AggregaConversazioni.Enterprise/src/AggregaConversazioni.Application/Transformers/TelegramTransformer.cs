using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public class TelegramTransformer : RegexBasedTransformer
{
    public TelegramTransformer()
    {
        // Converti formato Telegram: Nome, [DD/MM/YYYY HH:MM] → '''Nome: '''
        RegexReplacements.Add((@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n?", "'''$1: '''"));
    }
}
