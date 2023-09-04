using System.Text.RegularExpressions;

namespace AggregaConversazioni;

public class RegexDescription
{
    public Regex Regex { get; set; }
    public string Description { get; set; }
    public string To { get; set; }
    public string From => Regex.ToString();

    public RegexDescription(string regex, string description, string to)
    {
        Regex = new Regex(regex);
        Description = description;
        To = to;
    }

    public RegexDescription((string from, string to) regex, string description = "")
    {
        Regex = new Regex(regex.from);
        Description = description;
        To = regex.to;
    }

    public string Replace(string input)
    {
        return Regex.Replace(input, To);
    }
}