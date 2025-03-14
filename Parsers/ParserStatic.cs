using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Parsers
{
    internal static class ParserStatic
    {
        public static List<string> GetMostFreqLines(IEnumerable<string> lines)
        {
            return lines.GroupBy(s => s)
                .Select(group => new {
                    Text = group.Key,
                    Count = group.Count()
                })
                .Where(l => l.Count != 1
                            && !string.IsNullOrWhiteSpace(l.Text)
                            && l.Text.Length < 20)
                .OrderByDescending(x => x.Count)
                .Select(s => s.Text).ToList();
        }

        public static string ParseIo_LeiCiclico(string text)
        {
            //versione vecchia
            //Regex regexObj2 = new Regex(@"^(Io|Lei): ([^:]+?)[\n\s\r]+\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            Regex regexObj2 = new Regex(@"^(Io|Lei): ((?:(?!^(Io|Lei)).)+?)[\n\s\r]+\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

            string prima = text, dopo;


            bool hasChanged;
            do
            {
                dopo = regexObj2.Replace(prima, @"$1: $2 ");
                hasChanged = prima != dopo;
                prima = dopo;
            } while (hasChanged);

            return dopo;
        }

        internal static Regex ExecuteRegex(string search)
        {
            return new Regex(search, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }
}
