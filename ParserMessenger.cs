using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni
{
    internal class ParserMessenger : Parser
    {
        public override (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) Parse(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

            //Cerco quelli con Immagine del profilo di 
            string search = "^(.+?) Immagine del profilo di";
            var speakers = IdentifySpeakers(enumerable, search);

            var lines2 = ApplyRegex(ref text, enumerable);

            var k = IdentifySpeaker2(enumerable);

            return (text, k, speakers);
        }


        public IEnumerable<string> ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            string longSpeakerName = "Petra Giuliani";
            //shortSpeakerName = "Julia";
            var shortSpeakerName = longSpeakerName.Split(' ').First();


            List<(string from, string to)> regexes = new List<(string @from, string to)>
            {
                ($"You unsent a message[\n\r]+", ""),
                ($"{shortSpeakerName}[\n\r]+{longSpeakerName}[\n\r]+", "Lei: "),
                (@"You sent[\n\r]+", "Io: "),
                ($"{shortSpeakerName} replied to you[\n\r]+", "Io: "),
                ($"You replied to {shortSpeakerName}[\n\r]+", "Io: "),
                ($"{shortSpeakerName} replied to themself[\n\r]+", "Lei: "),
                ($"{shortSpeakerName}[\n\r]+", "Lei: "),
                ($"{longSpeakerName}[\n\r]+", "Lei: "),

                //elimino da Noted
                ($"^Enter[\n\r]+", ""),

                //elimino le ore 
                (@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""),
            };

            //Sempre per Noted, elimino stringhe tipo queste:
            //### You replied to Petra
            //### You sent
            //### Petra replied to you
            //### Petra replied to themself
            //### Petra
            regexes = regexes.Select(r => ("^### " + r.from, r.to)).Union(regexes).ToList();

            DebugOutputTable = DebugHelper.Annotate(text, regexes);

            string text1 = text;
            foreach (var reg in regexes)
            {
                text1 = Regex.Replace(text1, reg.from + "\n?", reg.to,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            }

            text = text1;

            //Parse Io/Lei ciclico
            text = ParseIo_LeiCiclico(text);

            return lines;
        }
    }
}
