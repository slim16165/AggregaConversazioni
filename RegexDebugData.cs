namespace AggregaConversazioni;

/// <summary>
/// Represents the details of a regex replacement operation.
/// </summary>
public class RegexDebugData
{
    /// <summary>
    /// The source regex pattern
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// The replacement string for the regex pattern
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// Total number of replacements made using the regex pattern
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Number of distinct replacements made using the regex pattern
    /// </summary>
    public int CountDistinct { get; set; }
}
