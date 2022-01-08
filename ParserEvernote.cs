using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

namespace AggregaConversazioni
{
    class ParserEvernote : Parser
    {
        public static string AnalizzaEvernote(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
            //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

            var lines2 = ApplyRegex(ref text, enumerable);

            return text;
        }

        public static IEnumerable<string> ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            text = Regex.Replace(text, $"You unsent a message[\n\r]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            
            //elimino le ore 
            text = Regex.Replace(text, @"^\d{1,2}:\d{2} [ap]m[\n\r]", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Regex regexObj2 = new Regex(@"^(Io|Lei): (.*)\n\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            for (int i = 0; i < 10; i++)
            {
                text = regexObj2.Replace(text, @"$1: $2 ");
            }

            return lines;
        }
    }
}
