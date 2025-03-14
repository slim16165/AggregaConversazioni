using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Transformers;

public class MarkdownToWikiConverterPandoc : ITextTransformer
{
    public string Transform(string input)
    {
        // Trova il percorso dell'eseguibile Pandoc fornito da Pandoc.Windows
        string pandocPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pandoc.exe");

        pandocPath = "C:\\Users\\g.salvi\\AppData\\Local\\Pandoc\\pandoc.exe";

        if (!File.Exists(pandocPath))
        {
            throw new FileNotFoundException("Impossibile trovare Pandoc. Assicurati che il pacchetto Pandoc.Windows sia installato.");
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = pandocPath,
            Arguments = "--from=markdown --to=mediawiki",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
        };

        using (var process = new Process { StartInfo = processInfo })
        {
            process.Start();
            
            // Scriviamo in UTF-8
            using (var writer = new StreamWriter(
                       process.StandardInput.BaseStream,
                       new UTF8Encoding(false)))  // "false" per non aggiungere BOM
            {
                writer.Write(input);
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Errore durante la conversione con Pandoc: {error}");
            }

            return output;
        }
    }
}