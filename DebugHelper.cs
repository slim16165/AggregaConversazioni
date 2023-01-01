using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleTableExt;

namespace AggregaConversazioni
{
    internal static class DebugHelper
    {
        public static string Annotate(string text1, List<(string from, string to)> regexes)
        {
            //Console.WriteLine("| from | to | sostituzioni |");
            //Console.WriteLine("|------|----|--------------|");
            //var enumerable = sostituzioni.Select(s => $"| {s.Source} | {s.Result} |");

            var tableData = (
                from reg in regexes 
                let sost = CalculateReplacement(text1, reg) 
                select new List<object>
                {
                    Escape(reg.@from), 
                    Escape(reg.to), 
                    sost.count,
                    sost.countDist,
                    //sost.sost
                }).ToList();


            var tableRender =
                ConsoleTableBuilder
                    .From(tableData)
                    .WithColumn("Regex from", "Regex to"/*, "sostituzioni"*/)
                    .WithPaddingLeft(string.Empty)
                    .WithFormat(ConsoleTableBuilderFormat.Default)
                    .Export();


            return tableRender.ToString();
        }

        private static (string sost, int count, int countDist) CalculateReplacement(string text1, (string from, string to) reg)
        {
            var matches = Regex.Matches(text1, reg.from,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

            var sostituzioniEffettuate = (from Match match in matches
                select new
                {
                    Source = match.Value,
                    Result = match.Result(reg.to)
                }).ToList();

            var count = sostituzioniEffettuate.Count();

            var sostEffettuateDist =
                sostituzioniEffettuate.Distinct().ToList();

            var countDist = sostEffettuateDist.Count;

            var sost = sostEffettuateDist.Any()
                ? sostEffettuateDist.Select(s => $"{Escape(s.Source)} → {Escape(s.Result)}")
                    .Aggregate((a, b) => a + "\r" + b)
                : "";
            
            return (sost, count, countDist);
        }

        private static string Escape(string source)
        {
            source = source.Replace("\n", "");
            source = source.Replace("\r", "");
            return source.Substring(0, Math.Min(source.Length, 30));
        }
    }
}