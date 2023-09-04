using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Parser;

public abstract class ParserBase
{
    protected ParserContext Context { get; } = new ParserContext();

    // Table for debugging purposes
    public static List<string> Speakers { get; set; }


    // Abstract method to parse the text and return text, a set of lines, and identified speakers
    public abstract (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text);
    
    public ParserBase WithText(string originalText)
    {
        Context.OriginalText = originalText;
        return this;
    }

    public ParserBase SplitTextIntoLines()
    {
        Context.TextLines = Regex.Split(Context.OriginalText, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        return this;
    }

    public (string, IEnumerable<RigaDivisaPerPersone>, List<string>) GetResult()
    {
        // TODO: Return appropriate values based on the Context
        return (Context.OriginalText, null, null); // Temporary return value, adjust as necessary
    }

    public abstract ParserBase IdentifySpeakers();
    public abstract ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null);

    public ParserBase ApplyRegexAndClean()
    {
        ApplyAllPatterns();
        ApplyPatternsWithNewLines();
        AnnotateDebugOutput();
        ApplyPostProcessingPatterns();

        return this;
    }

    protected virtual void ApplyAllPatterns()
    {
        foreach (var regexPattern in Context.RegexGroups)
        {
            Context.OriginalText = regexPattern.ApplyAll(Context.OriginalText);
        }
    }

    protected virtual void ApplyPatternsWithNewLines()
    {
        var regexReplacements = Context.RegexGroups.SelectMany(group => group.RegexRules).ToList();
        regexReplacements = RegexReplacementsWithNewLine(regexReplacements);

        foreach (var pattern in regexReplacements)
        {
            Context.OriginalText = Regex.Replace(Context.OriginalText, pattern.From + "\n?", pattern.To,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }
    }

    public List<RegexDescription> RegexReplacementsWithNewLine(List<RegexDescription> regexReplacements)
    {
        var regexWithPotentialNewLine = regexReplacements.Select(pattern =>
            new RegexDescription((pattern.From + Environment.NewLine, pattern.To), pattern.Description));

        regexReplacements = regexWithPotentialNewLine.Union(regexReplacements).ToList();
        return regexReplacements;
    }

    protected virtual void AnnotateDebugOutput()
    {
        var regexReplacements = Context.RegexGroups.SelectMany(group => group.RegexRules).ToList();
        Context.DebugOutputTable = DebugHelper.Annotate(Context.OriginalText, regexReplacements);
    }

    protected virtual void ApplyPostProcessingPatterns()
    {
        //...
    }
}
