using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public class EvernoteTransformer : RegexBasedTransformer
{
    public EvernoteTransformer()
    {
        // Rimuovi link ai gruppi Facebook
        RegexReplacements.Add((@"https://.{1,4}facebook\.com/groups/[^""<>\s]+", ""));

        // Rimuovi classi CSS
        RegexReplacements.Add((@"\s*class=""[^""<>]+""", ""));

        // Rimuovi undefined
        RegexReplacements.Add((@"\bundefined\b", ""));

        // Rimuovi attributi style
        RegexReplacements.Add((@"style=""--[^\""]+""", ""));
        RegexReplacements.Add((@"style=""font-family[^\""]+""", ""));

        // Rimuovi span vuoti
        RegexReplacements.Add((@"<(span)\s*>(.*?)</\k<1>>", "$2"));

        // Rimuovi tag inutili
        RegexReplacements.Add((@"^\s*</?(en-note|body|html|meta|input) *>[\s\n\r]*", ""));

        // Pulisci link Facebook (fbclid)
        RegexReplacements.Add((@"\??fbclid=\w+(?=\W)", ""));

        // Converti div in newline
        RegexReplacements.Add((@"</?div>", "\n"));

        // Converti link HTML in formato MediaWiki
        RegexReplacements.Add((@"<a href=\""([^\""]+)\""[^<>]>(.*?)</a>", "[$1|$2]"));

        // Converti HR in sezioni
        RegexReplacements.Add((@"<hr/?>", "== Titolo sezione =="));

        // Rimuovi troppi a capo
        RegexReplacements.Add((@"[\n|\r]+", "\n"));
    }
}
