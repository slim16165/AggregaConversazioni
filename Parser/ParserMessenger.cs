using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Parser;
using static System.Environment;

namespace AggregaConversazioni;

internal class ParserMessenger : ParserBase
{
    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string originalText)
    {
        // Split the original text into individual lines and trim any leading or trailing whitespace from each line
        var textLines = Regex.Split(originalText, @"(\n|\r)+").Select(o => o.Trim()).ToList();

        // Search for lines containing "Immagine del profilo di" to identify potential speakers
        string profileImagePattern = "^(.+?) Immagine del profilo di";
        var speakersFromPattern = IdentifySpeakersBySearchString(textLines, profileImagePattern);

        // Use a secondary method to identify speakers
        var speakersFromMethod2 = IdentifySpeaker2(textLines);

        // Process the original text based on certain regex patterns
        var processedLines = ApplyRegexAndClean(ref originalText, textLines);

        return (originalText, speakersFromMethod2, speakersFromPattern);
    }



    public IEnumerable<string> ApplyRegexAndClean(ref string originalText, IEnumerable<string> lines)
    {
        // Defining speaker names for better recognition
        const string FullSpeakerName = "Laura Eileen Gallo";
        var ShortSpeakerName = FullSpeakerName.Split(' ').First();

        // Set of regex patterns and their replacements with corresponding description
        List<RegexDescription> regexReplacements = new List<RegexDescription>
    {
        /*FROM - TO - Description*/
        new(($"You unsent a message", ""),                        "Eliminate the literal message: 'You unsent a message'"),
        new(($"{ShortSpeakerName}{FullSpeakerName}", "Lei: "),    "Replace short and full speaker names with 'Lei: '"),
        new(($"You sent", "Io: "),                                "Replace 'You sent' with 'Io: '"),
        new(($"{ShortSpeakerName} replied to you", "Io: "),       $"Replace '{ShortSpeakerName} replied to you' with 'Io: '"),
        new(($"You replied to {ShortSpeakerName}", "Io: "),       $"Replace 'You replied to {ShortSpeakerName}' with 'Io: '"),
        new(($"{ShortSpeakerName} replied to themself", "Lei: "), $"Replace '{ShortSpeakerName} replied to themself' with 'Lei: '"),
        new(($"{ShortSpeakerName}", "Lei: "),                     $"Replace '{ShortSpeakerName}' with 'Lei: '"),
        new(($"{FullSpeakerName}", "Lei: "),                      $"Replace '{FullSpeakerName}' with 'Lei: '"),

        // Specific rules for the application "Noted"
        new(($"^Enter", ""),                                      "Eliminate 'Enter' from the start of lines"),
        new((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""),                  "Eliminate time-stamp hours from the start of lines"),
    };

        // Apply all the regex replacements on the original text
        string cleanedText = originalText;
        foreach (var regexPattern in regexReplacements)
        {
            cleanedText = regexPattern.Regex.Replace(cleanedText, regexPattern.To);
        }

        // Apply regex patterns that could be followed by any new line
        //Sempre per Noted, elimino stringhe tipo queste:
        //### You replied to Petra
        //### You sent
        //### Petra replied to you
        //### Petra replied to themself
        //### Petra
        var regexWithPotentialNewLine = regexReplacements.Select(pattern =>
            new RegexDescription((pattern.From + NewLine, pattern.To), pattern.Description));

        regexReplacements = regexWithPotentialNewLine.Union(regexReplacements).ToList();

        // Annotate the text for debugging purposes
        DebugOutputTable = DebugHelper.Annotate(originalText, regexReplacements);

        // Apply all regex replacements including those with potential new lines
        foreach (var pattern in regexReplacements)
        {
            originalText = Regex.Replace(originalText, pattern.From + "\n?", pattern.To,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        // Additional parsing logic for cyclic Io/Lei patterns
        originalText = ParserStatic.ParseIo_LeiCiclico(originalText);

        return lines;
    }

}