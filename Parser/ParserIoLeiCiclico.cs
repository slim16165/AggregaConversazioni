using System.Collections.Generic;
using AggregaConversazioni.Parser;

namespace AggregaConversazioni;

public class ParserIoLeiCiclico : ParserBase
{
    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        text = ParserStatic.ParseIo_LeiCiclico(text);
        return (text, null, null);
    }

    public override ParserBase IdentifySpeakers()
    {
        return this;
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        return this;
    }
}