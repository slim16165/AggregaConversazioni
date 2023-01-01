using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TextTableFormatter;

namespace AggregaConversazioni
{
    internal class DebugHelper
    {
        public static string Annotate(string text1, List<(string from, string to)> regexes)
        {
            //Console.WriteLine("| from | to | sostituzioni |");
            //Console.WriteLine("|------|----|--------------|");
            //var enumerable = sostituzioni.Select(s => $"| {s.Source} | {s.Result} |");

            var basicTable = new TextTable(3);
            basicTable.AddCell("Regex from");
            basicTable.AddCell("Regex to");
            basicTable.AddCell("sostituzioni");

            foreach (var reg in regexes)
            {
                var matches = Regex.Matches(text1, reg.from,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

                var sostituzioni =
                    (from Match match in matches
                        select new
                        {
                            Source = match.Value,
                            Result = match.Result(reg.to)
                        }).Distinct().ToList();

                basicTable.AddCell(Escape(reg.from));
                basicTable.AddCell(Escape(reg.to));
                foreach (var s in sostituzioni)
                {
                    basicTable.AddCell($"{Escape(s.Source)} → {Escape(s.Result)}");
                }
            }

            var tableRender = basicTable.Render();
            Console.WriteLine(tableRender);
            return tableRender;
        }

        private static string Escape(string source)
        {
            source = source.Replace("\n", "\\n");
            source = source.Replace("\r", "\\r");
            return source;
        }
    }
}