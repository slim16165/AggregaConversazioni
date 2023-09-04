using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni;

class ParserInstagram : Parser.ParserBase
{
    protected static string ApplyRegex(ref string text, string speaker)
    {
        List<(string from, string to)> regexes = new List<(string @from, string to)>()
        {
            // regexGiornoEOra =
            (@"(Lunedì|Martedì|Mercoledì|Giovedì|Sabato|Domenica) \d{1,2}:\d{2}", ""),
            //regexUsernameInterloc
            ("^Immagine del profilo di (.+?)$", "Lei: "),
            //Elimino le ore
            (@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""),
            (@"Annulla l'invio del messaggio", ""),
            (@"Mi piace", ""),
            (@"Copia", ""),
            (@"❤️", ""),
            //
            //@"^(Io|Lei): (.*)\n\k<1>: ", ""
        };

        string text1 = text;
        foreach (var (from, to) in regexes)
        {
            text1 = Regex.Replace(text1, from + "\n?", to,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        text = text1;

        return text;
    }


    public override (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

        //Cerco quelli con Immagine del profilo di 
        string search = "^Immagine del profilo di (.+?)$";
        var speakers = IdentifySpeakers(enumerable, search);
        var speaker = speakers.Single();

        var lines2 = ApplyRegex(ref text, speaker);

        var k = IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }
}