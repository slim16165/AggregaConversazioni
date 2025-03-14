using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;
using ConsoleTableExt;

namespace AggregaConversazioni.Helpers;

/// <summary>
/// Provides helper methods for debugging with regex patterns.
/// </summary>
internal static class DebugHelper
{
    /// <summary>
    /// Annotates a text with given regex patterns and their replacement values.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="regexes">A list of regex patterns and their corresponding replacements.</param>
    /// <returns>The annotated text.</returns>
    public static string Annotate(string text, List<RegexDescription> regexes)
    {
        var res = Annotate(text, regexes.Select(r => (r.From, r.To)).ToList());
        return res;
    }

    /// <summary>
    /// Annotates a text with given source strings and their replacement values.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="regexes">A list of source strings and their corresponding replacements.</param>
    /// <returns>The annotated text.</returns>
    public static string Annotate(string text, List<(string from, string to)> regexes)
    {
        // Calcola e restituisce la stringa annotata
        return CreateAnnotatedString(text, regexes);
    }

    // Il metodo PrepareAnnotationDataForUI resta disponibile per essere usato
    // dal ViewModel se necessario, ad esempio:
    public static ObservableCollection<RegexDebugData> GetAnnotationData(string text, List<(string from, string to)> regexes)
    {
        return PrepareAnnotationDataForUI(text, regexes);
    }


    public static string CreateAnnotatedString(string text, List<(string from, string to)> replacementRules)
    {
        //Console.WriteLine("| from | to | sostituzioni |");
        //Console.WriteLine("|------|----|--------------|");
        //var enumerable = sostituzioni.Select(s => $"| {s.Source} | {s.Result} |");

        var tableData = replacementRules.Select(rule =>
        {
            var data = CalculateReplacement(text, rule);
            return new List<object>
            {
                Escape(rule.from),
                Escape(rule.to),
                data.count,
                data.countDist
            };
        }).ToList();

        var tableRender = ConsoleTableBuilder
            .From(tableData)
            .WithColumn("Regex from", "Regex to")
            .WithPaddingLeft(string.Empty)
            .WithFormat(ConsoleTableBuilderFormat.Default)
            .Export();

        return tableRender.ToString();
    }

    public static ObservableCollection<RegexDebugData> PrepareAnnotationDataForUI(string text, List<(string from, string to)> regexes)
    {
        var tableDataViewModel = (
            from reg in regexes
            let replacementData = CalculateReplacement(text, reg)
            select new RegexDebugData
            {
                From = Escape(reg.@from),
                To = Escape(reg.to),
                Count = replacementData.count,
                CountDistinct = replacementData.countDist,
            }).ToList();

        return new ObservableCollection<RegexDebugData>(tableDataViewModel);
    }


    /// <summary>
    /// Calculates the replacement operations on a given text for a specific regex pattern.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="reg">The regex pattern and its replacement.</param>
    /// <returns>The details of the replacement operations.</returns>
    private static (string replacementDetails, int count, int countDist) CalculateReplacement(string text, (string from, string to) reg)
    {
        // 1. Trova tutte le corrispondenze del pattern regex nel testo fornito.
        var matches = Regex.Matches(text, reg.from, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        // 2. Per ogni corrispondenza trovata, crea un nuovo oggetto anonimo che contiene la stringa originale 
        // e il risultato previsto della regex replace
        var executedReplacements = (from Match match in matches
            select new
            {
                Source = match.Value,
                Result = match.Result(reg.to)
            }).ToList();

        // 3. Conta il numero totale di corrispondenze (e sostituzioni previste) trovate.
        var count = executedReplacements.Count;

        // 4. Filtra l'elenco di tutte le sostituzioni previste per rimuovere le duplicazioni e conta le sostituzioni uniche.
        var distinctExecutedReplacements = executedReplacements.Distinct().ToList();
        var countDist = distinctExecutedReplacements.Count;

        // 5. Crea una rappresentazione dettagliata delle sostituzioni. Se ci sono sostituzioni uniche da effettuare, 
        // combina tutte queste sostituzioni in una stringa. La stringa avrà un formato in cui ogni sostituzione 
        // originale e il suo risultato sono separati da una freccia e ogni coppia è separata da un ritorno a capo.
        var replacementDetails = distinctExecutedReplacements.Any()
            ? distinctExecutedReplacements.Select(s => $"{Escape(s.Source)} → {Escape(s.Result)}")
                .Aggregate((a, b) => a + "\r" + b)
            : "";

        // 6. Ritorna i dettagli delle sostituzioni, il conteggio totale e il conteggio distinto.
        return (replacementDetails, count, countDist);

    }

    /// <summary>
    /// Escapes specific characters in a given string.
    /// </summary>
    private static string Escape(string source)
    {
        //source = source.Replace("\n", "");
        //source = source.Replace("\r", "");
        return source; //.Substring(0, Math.Min(source.Length, 30));
    }
}