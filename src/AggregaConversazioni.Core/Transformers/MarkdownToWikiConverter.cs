using System.Text.RegularExpressions;

namespace AggregaConversazioni.Transformers;

public class MarkdownToWikiConverter : RegexBasedTransformer
{
    public MarkdownToWikiConverter()
    {
        // Rimuovi spazi bianchi finali da tutte le righe
        RegexReplacements.Add((@"\s+$", ""));

        // Converti titoli di livello 1 a 6
        RegexReplacements.Add((@"^# (.+)$", "== $1 =="));
        RegexReplacements.Add((@"^## (.+)$", "=== $1 ==="));
        RegexReplacements.Add((@"^### (.+)$", "==== $1 ===="));
        RegexReplacements.Add((@"^#### (.+)$", "===== $1 ====="));
        RegexReplacements.Add((@"^##### (.+)$", "====== $1 ======"));
        RegexReplacements.Add((@"^###### (.+)$", "======= $1 ======="));

        // Converti grassetto: **testo** e __testo__
        RegexReplacements.Add((@"\*\*(.+?)\*\*", "'''$1'''"));
        RegexReplacements.Add((@"__(.+?)__", "'''$1'''"));

        // Converti corsivo: *testo* e _testo_
        RegexReplacements.Add((@"\*(.+?)\*", "''$1''"));
        RegexReplacements.Add((@"_(.+?)_", "''$1''"));

        // Converti link: [testo](url) e [testo](url "titolo")
        RegexReplacements.Add((@"\[(.+?)\]\((.+?) ""(.+?)""\)", "[[$2|$1]]")); // Con titolo
        RegexReplacements.Add((@"\[(.+?)\]\((.+?)\)", "[[$2|$1]]")); // Senza titolo

        // Converti immagini: ![alt](url)
        RegexReplacements.Add((@"!\[(.+?)\]\((.+?)\)", "[[File:$2|$1]]"));

        // Converti liste non ordinate: - elemento
        RegexReplacements.Add((@"^- (.+)$", "* $1"));

        // Converti liste ordinate: 1. elemento
        RegexReplacements.Add((@"^\d+\. (.+)$", "# $1"));

        // Converti blocchi di codice: ```linguaggio\ncodice\n```
        RegexReplacements.Add((@"```(\w+)?\n([\s\S]+?)\n```", "<syntaxhighlight lang=\"$1\">\n$2\n</syntaxhighlight>"));

        // Converti codice inline: `codice`
        RegexReplacements.Add((@"`(.+?)`", "<code>$1</code>"));

        // Converti citazioni: > testo
        RegexReplacements.Add((@"^> (.+)$", "<blockquote>$1</blockquote>"));

        // Converti righe orizzontali: ---
        RegexReplacements.Add((@"^---$", "----"));

        // Gestisci paragrafi: due newline per separare paragrafi
        RegexReplacements.Add((@"\n\n", "\n\n"));
    }

    public override string Transform(string input)
    {
        // Applica le regex in ordine
        foreach (var (pattern, replacement) in RegexReplacements)
        {
            input = Regex.Replace(input, pattern, replacement, RegexOptions.Multiline);
        }

        return input;
    }
}