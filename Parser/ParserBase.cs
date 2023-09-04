using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Parser;

internal abstract class ParserBase
{
    public string DebugOutputTable = "";

    public abstract (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) Parse(string text);

    public static List<string> IdentifySpeakers(IEnumerable<string> lines, string search)
    {
        var reg = ParserStatic.ExecuteRegex(search);
        var k = lines.GetCapturingGroup(reg).DistinctNotEmpty();

        //Metodo alternativo, cerco le righe ripetute più volte
        var mostFreqLines = ParserStatic.GetMostFreqLines(lines);

        return k;
    }

    protected static IEnumerable<RigaDivisaPerPersone> IdentifySpeaker2(IEnumerable<string> lines)
    {
        //Elimino le righe vuote
        lines = lines.Where(o => !string.IsNullOrWhiteSpace(o));

        // Dictionary to hold the lines for each speaker
        Dictionary<string, List<string>> speakersDict = new Dictionary<string, List<string>>();

        string currentSpeaker = null;

        foreach (string curLine in lines)
        {
            // If current line is a speaker's name
            if (IsSpeaker(curLine)) // You'll need to implement this function
            {
                currentSpeaker = curLine;
                if (!speakersDict.ContainsKey(currentSpeaker))
                {
                    speakersDict[currentSpeaker] = new List<string>();
                }
            }
            else if (currentSpeaker != null)
            {
                speakersDict[currentSpeaker].Add(curLine);
                var row = new RigaDivisaPerPersone();
                row.GetType().GetProperty(currentSpeaker).SetValue(row, curLine);
                yield return row;
            }
        }
    }

    // Simple function to determine if a line is a speaker
    private static bool IsSpeaker(string line)
    {
        // Here, add logic to determine if a line is a speaker.
        // This could be a list of known speakers, or some other criteria.
        return ...;
    }


    public virtual List<string> ExtractSpeakers(string text)
    {
        // Pattern to identify names or names with surnames followed by a newline.
        Regex regex = new Regex(@"^(\w+)( \w+)?$", RegexOptions.Multiline);

        var matches = regex.Matches(text);

        // Extract the matched speakers and convert them to a list
        List<string> speakers = matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();

        return speakers;
    }
}