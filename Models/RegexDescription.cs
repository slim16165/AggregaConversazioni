using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Models;

public enum RegexCategory
{
    GeneralCleanup,
    SpeakerYouReplacement,
    SpeakerOtherReplacement,
    ReplyBasedReplacement
}

public class RegexDescription
{
    public Regex Regex { get; private set; }
    public string Description { get; set; }
    public string To { get; set; }
    public string From => Regex.ToString();
    public RegexCategory Category { get; set; } // Added categorization
    public bool IsOrderImportant { get; set; }  // A flag to check if the order of application is important

    public RegexDescription((string from, string to) regex, string description = "", RegexCategory category = RegexCategory.GeneralCleanup, bool isOrderImportant = false)
    {
        Regex = new Regex(regex.from);
        Description = description;
        To = regex.to;
        Category = category;
        IsOrderImportant = isOrderImportant;
    }

    public string Replace(string input)
    {
        return Regex.Replace(input, To);
    }
}

public class RegexGroup
{
    public List<RegexDescription> RegexRules { get; set; } = new List<RegexDescription>();
    public bool IsOrderImportant { get; set; } // Indica se l'ordine delle regex nel gruppo è importante
    public string GroupName { get; set; } // Una descrizione o un nome per il gruppo, opzionale

    public RegexGroup(string groupName, bool isOrderImportant = false)
    {
        GroupName = groupName;
        IsOrderImportant = isOrderImportant;
    }

    // Potresti anche avere un metodo per applicare tutte le regex del gruppo a una stringa di input
    public string ApplyAll(string input)
    {
        foreach (var rule in RegexRules)
        {
            input = rule.Replace(input);
        }
        return input;
    }
}
