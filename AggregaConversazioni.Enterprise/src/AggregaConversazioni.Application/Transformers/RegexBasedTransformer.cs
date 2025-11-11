using System.Text.RegularExpressions;

namespace AggregaConversazioni.Application.Transformers;

public abstract class RegexBasedTransformer : ITextTransformer
{
    protected List<(string pattern, string replacement)> RegexReplacements { get; set; } = new();

    public virtual string Transform(string input)
    {
        foreach (var (pattern, replacement) in RegexReplacements)
        {
            input = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        return input;
    }
}
