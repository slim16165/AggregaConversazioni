using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

namespace AggregaConversazioni
{
    class ParserInstagram : Parser
    {
        public static (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) AnalizzaInstagram(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
            //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

            var speakers = Parser.IdentifySpeakers(enumerable);

            var lines2 = ApplyRegex(ref text, enumerable);

            var k = IdentifySpeaker2(enumerable);

            return (text, k, speakers);
        }

        protected static IEnumerable<string> ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            (string search, string replace) regexGiornoEOra =
                (search: @"(Lunedì|Martedì|Mercoledì|Giovedì|Sabato|Domenica) \d{1,2}:\d{2}", replace: "");

            (string search, string replace) regexUsernameInterloc =
                (search: "^Immagine del profilo di ([^ ]+)$", replace: "$1: ");

            var longSpeakerName = "Sara Stefanile";
            var shortSpeakerName = longSpeakerName.Split(' ').First();

            text = Regex.Replace(text, regexGiornoEOra.search + "[\n\r]+", regexGiornoEOra.replace, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+{longSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, @"You sent[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{shortSpeakerName} replied to you[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"You replied to {shortSpeakerName}[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{longSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);

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
