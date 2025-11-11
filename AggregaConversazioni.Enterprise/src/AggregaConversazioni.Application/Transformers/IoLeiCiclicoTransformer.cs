using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public class IoLeiCiclicoTransformer : RegexBasedTransformer
{
    public IoLeiCiclicoTransformer()
    {
        // Pattern per conversazioni cicliche Io/Lei
        // Questo è un placeholder - la logica completa dovrebbe essere migrata da ParserStatic.ParseIo_LeiCiclico
        RegexReplacements.Add((@"^Io:\s*(.+)$", "'''Io:''' $1"));
        RegexReplacements.Add((@"^Lei:\s*(.+)$", "'''Lei:''' $1"));
    }
}
