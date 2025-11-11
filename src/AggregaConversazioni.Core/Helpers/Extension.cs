using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Helpers;

static class Extension
{
    internal static List<string> GetCapturingGroup(this IEnumerable<string> lines, Regex regexObj)
    {
        return lines.Select(subjectString => regexObj.Match(subjectString).Groups[1].Value).ToList();
    }

    internal static List<string> DistinctNotEmpty(this IEnumerable<string> lines)
    {
        return lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
    }
}