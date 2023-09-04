using System.Collections.Generic;
using System.Linq;
using AggregaConversazioni.Parser;

namespace AggregaConversazioni;

public class ParserContext
{
    public string OriginalText { get; set; }
    public IEnumerable<string> TextLines { get; set; }
    public List<string> IdentifiedSpeakers { get; set; }
    public List<RegexGroup> RegexGroups { get; set; } = new List<RegexGroup>();
    public string DebugOutputTable { get; set; }
}

internal class ParserMessenger : ParserBase
{
    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string originalText)
    {
        var result = new ParserMessenger()
            .WithText(originalText)
            .SplitTextIntoLines()
            .IdentifySpeakers()
            .InitializeRegexPatterns("Laura Eileen Gallo")
            .ApplyRegexAndClean()
            .GetResult();

        //// Process the original text based on certain regex patterns
        //var processedLines = ApplyRegexAndClean(ref originalText, textLines, regexGroups);

        return result; //(originalText, speakersFromMethod2, speakersFromPattern);
    }

    protected override void ApplyPostProcessingPatterns()
    {
        Context.OriginalText = ParserStatic.ParseIo_LeiCiclico(Context.OriginalText);
    }

    public override ParserBase IdentifySpeakers()
    {
        // Search for lines containing "Immagine del profilo di" to identify potential speakers
        string profileImagePattern = "^(.+?) Immagine del profilo di";
        var speakersFromPattern = SpeakerIdentification.IdentifySpeakersBySearchString(Context.TextLines, profileImagePattern);

        // Add the identified speakers to the Context or use them however you need
        Context.IdentifiedSpeakers = speakersFromPattern;

        return this;
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        if (shortSpeakerName == null)
            shortSpeakerName = fullSpeakerName.Split(' ').First();

        // Set of regex patterns and their replacements with corresponding description
        var messageOperations = new RegexGroup("Message Operations")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"You unsent a message", ""), "Eliminate the literal message: 'You unsent a message'")
            }
        };

        var speakerNameReplacements = new RegexGroup("Speaker Name Replacements", true) // True indica che l'ordine è importante
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"{shortSpeakerName}{fullSpeakerName}", "Lei: "), "Replace short and full speaker names with 'Lei: '"),
                new(($"You sent", "Io: "), "Replace 'You sent' with 'Io: '"),
                new(($"{shortSpeakerName}", "Lei: "), $"Replace '{shortSpeakerName}' with 'Lei: '"),
                new(($"{fullSpeakerName}", "Lei: "), $"Replace '{fullSpeakerName}' with 'Lei: '"),
            }
        };

        var replyOperations = new RegexGroup("Reply Operations")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"{shortSpeakerName} replied to you", "Io: "), $"Replace '{shortSpeakerName} replied to you' with 'Io: '"),
                new(($"You replied to {shortSpeakerName}", "Io: "), $"Replace 'You replied to {shortSpeakerName}' with 'Io: '"),
                new(($"{shortSpeakerName} replied to themself", "Lei: "), $"Replace '{shortSpeakerName} replied to themself' with 'Lei: '")
            }
        };

        var appSpecificOperations = new RegexGroup("App Specific Operations - Noted")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"^Enter", ""), "Eliminate 'Enter' from the start of lines"),
                new((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""), "Eliminate time-stamp hours from the start of lines")
            }
        };

        List<RegexGroup> regexGroups = new List<RegexGroup>
        {
            messageOperations,
            speakerNameReplacements,
            replyOperations,
            appSpecificOperations
        };

        Context.RegexGroups = regexGroups;

        return this;
    }
}