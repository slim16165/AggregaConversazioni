using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

namespace AggregaConversazioni
{
    class ParserMessenger : Parser
    {
        public static (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) AnalizzaMessenger(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

            //Cerco quelli con Immagine del profilo di 
            string search = "^(.+?) Immagine del profilo di";
            var speakers = Parser.IdentifySpeakers(enumerable, search);

            var lines2 = ApplyRegex(ref text, enumerable);

            var k = IdentifySpeaker2(enumerable);

            return (text, k, speakers);
        }


        public static IEnumerable<string> ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            string shortSpeakerName;
            string longSpeakerName = "Stephanie Frogs";
            //shortSpeakerName = "Julia";
            longSpeakerName = "Julia Margini";
            longSpeakerName = "Francesco Arnaldi";
            longSpeakerName = "Sara Stefanile";
            shortSpeakerName = longSpeakerName.Split(' ').First();


            List<(string from, string to)> regexes = new List<(string @from, string to)>
            {
                ($"You unsent a message[\n\r]+", ""),
                ($"{shortSpeakerName}[\n\r]+{longSpeakerName}[\n\r]+", "Lei: "),
                (@"You sent[\n\r]+", "Io: "),
                ($"{shortSpeakerName} replied to you[\n\r]+", "Io: "),
                ($"You replied to {shortSpeakerName}[\n\r]+", "Io: "),
                ($"{shortSpeakerName}[\n\r]+", "Lei: "),
                ($"{longSpeakerName}[\n\r]+", "Lei: "),

                //elimino le ore 
                (@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""),
            };

            text = Parser.ApplyRegex(text, regexes);

            //Parse Io/Lei ciclico
            text = ParseIo_LeiCiclico(text);

            return lines;
        }
    }
}
