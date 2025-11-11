using System.Diagnostics;
using System.Text;

namespace AggregaConversazioni.Application.Transformers;

public class MarkdownToWikiPandocConverter : ITextTransformer
{
    private readonly string _pandocPath;

    public MarkdownToWikiPandocConverter(string? pandocPath = null)
    {
        _pandocPath = pandocPath ?? FindPandocPath();
    }

    private static string FindPandocPath()
    {
        // Cerca Pandoc in percorsi comuni
        var possiblePaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pandoc", "pandoc.exe"),
            "/usr/bin/pandoc",
            "/usr/local/bin/pandoc",
            "pandoc" // Se è nel PATH
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path) || path == "pandoc")
            {
                return path;
            }
        }

        throw new FileNotFoundException("Pandoc non trovato. Assicurati che Pandoc sia installato e nel PATH.");
    }

    public string Transform(string input)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = _pandocPath,
            Arguments = "--from=markdown --to=mediawiki",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();

        // Scriviamo in UTF-8
        using (var writer = new StreamWriter(
                   process.StandardInput.BaseStream,
                   new UTF8Encoding(false))) // "false" per non aggiungere BOM
        {
            writer.Write(input);
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error) && process.ExitCode != 0)
        {
            throw new Exception($"Errore durante la conversione con Pandoc: {error}");
        }

        return output;
    }
}
