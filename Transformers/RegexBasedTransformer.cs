using System.Collections.Generic;
using System.Text.RegularExpressions;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Transformers;

public abstract class RegexBasedTransformer : ITextTransformer
{
    protected List<(string pattern, string replacement)> RegexReplacements { get; set; } = new List<(string, string)>();

    public virtual string Transform(string input)
    {
        foreach (var (pattern, replacement) in RegexReplacements)
        {
            input = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        return input;
    }
}