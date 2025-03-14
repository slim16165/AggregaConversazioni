namespace AggregaConversazioni.Transformers;

public class MarkdownToWikiConverter : RegexBasedTransformer
{
    public MarkdownToWikiConverter()
    {
        // Converti titoli di livello 1: "# Titolo" → "== Titolo =="
        RegexReplacements.Add((@"^# (.+)$", "== $1 =="));

        // Converti titoli di livello 2: "## Titolo" → "=== Titolo ==="
        RegexReplacements.Add((@"^## (.+)$", "=== $1 ==="));

        // Converti il grassetto: **testo** → '''testo'''
        RegexReplacements.Add((@"\*\*(.+?)\*\*", "'''$1'''"));

        // Converti il corsivo: *testo* → ''testo''
        RegexReplacements.Add((@"\*(.+?)\*", "''$1''"));

        // Converti i link Markdown: [Testo](url) → [[url|Testo]]
        RegexReplacements.Add((@"\[(.+?)\]\((.+?)\)", "[[$2|$1]]"));

        // Converti le liste: "- Elemento" → "* Elemento"
        RegexReplacements.Add((@"^- (.+)$", "* $1"));
    }
}