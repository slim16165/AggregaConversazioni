using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

namespace AggregaConversazioni
{
    class ParserTelegram : Parser
    {
        public static (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) AnalizzaTelegram(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

            //Cerco quelli con Immagine del profilo di 
            string searchWithCapturingGroup = @"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n";
            var speakers = Parser.IdentifySpeakers(enumerable, searchWithCapturingGroup);

            var lines2 = ApplyRegex(ref text, enumerable);

            var k = IdentifySpeaker2(enumerable);

            return (text, k, speakers);
        }


        public static string ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            var longSpeakerName = "Sara Stefanile";
            var shortSpeakerName = longSpeakerName.Split(' ').First();

            List<(string from, string to)> regexes = new List<(string @from, string to)>()
            {
                //Sara Pinca, [16/01/2022 01:08] → '''Sara Pinca'''
                (@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
                //(@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
            };

            text = Parser.ApplyRegex(text, regexes);

            return text;

        }
    }
}
