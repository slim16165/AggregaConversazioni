using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni;

class ParserTelegram : Parser.ParserBase
{
    private static string ApplyRegex(ref string text, IEnumerable<string> lines)
    {
        var longSpeakerName = "Sara Stefanile";
        var shortSpeakerName = longSpeakerName.Split(' ').First();

        List<(string from, string to)> regexes = new List<(string @from, string to)>()
        {
            //Sara Pinca, [16/01/2022 01:08] → '''Sara Pinca'''
            (@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
            //(@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
        };

        string text1 = text;
        foreach (var reg in regexes)
        {
            text1 = Regex.Replace(text1, reg.from + "\n?", reg.to,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        text = text1;

        return text;

    }

    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

        //Cerco quelli con Immagine del profilo di 
        string searchWithCapturingGroup = @"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n";
        var speakers = IdentifySpeakersBySearchString(enumerable, searchWithCapturingGroup);

        var lines2 = ApplyRegex(ref text, enumerable);

        var k = IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }
}