using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Parser;
using static System.Environment;


namespace AggregaConversazioni;

internal class ParserMessenger : Parser.ParserBase
{
    public override (string text, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> speakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

        //Cerco quelli con Immagine del profilo di 
        string search = "^(.+?) Immagine del profilo di";
        var speakers = IdentifySpeakers(enumerable, search);

        var lines2 = ApplyRegex(ref text, enumerable);

        var k = IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }


    public IEnumerable<string> ApplyRegex(ref string originalText, IEnumerable<string> lines)
    {
        string longSpeakerName = "Laura Eileen Gallo";
        var shortSpeakerName = longSpeakerName.Split(' ').First();

        List<RegexDescription> regexes = new List<RegexDescription>
        {
            new(($"You unsent a message", ""), "Eliminate a literal message you unsente a message"),
            new(($"{shortSpeakerName}{longSpeakerName}", "Lei: "), "Replace short and long speaker name with 'Lei: '"),
            new(($"You sent", "Io: "), "Replace 'You sent' with 'Io: '"),
            new(($"{shortSpeakerName} replied to you", "Io: "), "Replace '{shortSpeakerName} replied to you' with 'Io: '"),
            new(($"You replied to {shortSpeakerName}", "Io: "), "Replace 'You replied to {shortSpeakerName}' with 'Io: '"),
            new(($"{shortSpeakerName} replied to themself", "Lei: "), "Replace '{shortSpeakerName} replied to themself' with 'Lei: '"),
            new(($"{shortSpeakerName}", "Lei: "), "Replace '{shortSpeakerName}' with 'Lei: '"),
            new(($"{longSpeakerName}", "Lei: "), "Replace '{longSpeakerName}' with 'Lei: '"),
                
            //For Noted
            new(($"^Enter", ""), "Eliminate 'Enter' from the beginning of the line"),
            new((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""), "Eliminate time stamp hours from the beginning of the line"),
        };

        string cleanedText = originalText;
        foreach (var regexDescription in regexes)
        {
            cleanedText = regexDescription.Regex.Replace(cleanedText, regexDescription.To);
        }



        //Sempre per Noted, elimino stringhe tipo queste:
        //### You replied to Petra
        //### You sent
        //### Petra replied to you
        //### Petra replied to themself
        //### Petra
        var regexesWithAnyLine = regexes.Select(r =>
            new RegexDescription((r.From + NewLine, r.To), r.Description));

        regexes = regexesWithAnyLine.Union(regexes).ToList();

        DebugOutputTable = DebugHelper.Annotate(originalText, regexes);

        string text1 = originalText;
        foreach (var reg in regexes)
        {
            text1 = Regex.Replace(text1, reg.From + "\n?", reg.To,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        originalText = text1;

        //Parse Io/Lei ciclico
        originalText = ParserStatic.ParseIo_LeiCiclico(originalText);

        return lines;
    }
}