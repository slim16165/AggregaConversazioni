using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleTableExt;

namespace AggregaConversazioni
{
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
            Annotate(text, regexes.Select(r => (r.From, r.To)).ToList());
            return text;
        }

        /// <summary>
        /// Annotates a text with given source strings and their replacement values.
        /// </summary>
        /// <param name="text1">The input text.</param>
        /// <param name="regexes">A list of source strings and their corresponding replacements.</param>
        /// <returns>The annotated text.</returns>
        public static string Annotate(string text1, List<(string from, string to)> regexes)
        {
        //Console.WriteLine("| from | to | sostituzioni |");
        //Console.WriteLine("|------|----|--------------|");
        //var enumerable = sostituzioni.Select(s => $"| {s.Source} | {s.Result} |");

            var tableData = (
                from reg in regexes
                let replacementData = CalculateReplacement(text1, reg)
                select new List<object>
                {
                    Escape(reg.@from),
                    Escape(reg.to),
                    replacementData.count,
                    replacementData.countDist,
                }).ToList();

            var tableDataViewModel = (
                from reg in regexes
                let replacementData = CalculateReplacement(text1, reg)
                select new RegexDebugData
                {
                    From = Escape(reg.@from),
                    To = Escape(reg.to),
                    Count = replacementData.count,
                    CountDist = replacementData.countDist,
                }).ToList();

            MainWindow.RegexDebug = new ObservableCollection<RegexDebugData>(tableDataViewModel);
            MainWindow.debugGrid2.ItemsSource = new ObservableCollection<RegexDebugData>(tableDataViewModel);

            var tableRender =
                ConsoleTableBuilder
                    .From(tableData)
                    .WithColumn("Regex from", "Regex to")
                    .WithPaddingLeft(string.Empty)
                    .WithFormat(ConsoleTableBuilderFormat.Default)
                    .Export();

            return tableRender.ToString();
        }

        /// <summary>
        /// Calculates the replacement operations on a given text for a specific regex pattern.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="reg">The regex pattern and its replacement.</param>
        /// <returns>The details of the replacement operations.</returns>
        private static (string replacementDetails, int count, int countDist) CalculateReplacement(string text, (string from, string to) reg)
        {
            var matches = Regex.Matches(text, reg.from, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

            var executedReplacements = (from Match match in matches
                                        select new
                                        {
                                            Source = match.Value,
                                            Result = match.Result(reg.to)
                                        }).ToList();

            var count = executedReplacements.Count();

            var distinctExecutedReplacements = executedReplacements.Distinct().ToList();
            var countDist = distinctExecutedReplacements.Count;

            var replacementDetails = distinctExecutedReplacements.Any()
                ? distinctExecutedReplacements.Select(s => $"{Escape(s.Source)} → {Escape(s.Result)}")
                    .Aggregate((a, b) => a + "\r" + b)
                : "";

            return (replacementDetails, count, countDist);
        }

        /// <summary>
        /// Escapes specific characters in a given string.
        /// </summary>
        /// <param name="source">The input string.</param>
        /// <returns>The escaped string.</returns>
        private static string Escape(string source)
        {
            return source;
        }
    }

    /// <summary>
    /// Represents the details of a regex replacement operation.
    /// </summary>
    public class RegexDebugData
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Count { get; set; }
        public int CountDist { get; set; }
    }
}
