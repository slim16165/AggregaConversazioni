using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;
using AggregaConversazioni.Utils;

namespace AggregaConversazioni.Parsers;

class ParserInstagram : ParserBase
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


    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

        //Cerco quelli con Immagine del profilo di 
        string search = "^Immagine del profilo di (.+?)$";
        var speakers = SpeakerIdentification.IdentifySpeakersBySearchString(enumerable, search);
        var speaker = speakers.Single();

        var lines2 = ApplyRegex(ref text, speaker);

        var k = SpeakerIdentification.IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }

    public override ParserBase IdentifySpeakers()
    {
        throw new System.NotImplementedException();
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        throw new System.NotImplementedException();
    }
}