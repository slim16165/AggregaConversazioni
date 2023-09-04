using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Parser;

public abstract class ParserBase
{
    // Table for debugging purposes
    public string DebugOutputTable = "";
    public List<string> Speakers { get; set; }

    // Abstract method to parse the text and return text, a set of lines, and identified speakers
    public abstract (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text);

    // Identify speakers based on a given regex pattern
    [Obsolete("Preferisco un sistema automatico")]
    public static List<string> IdentifySpeakersBySearchString(IEnumerable<string> lines, string search)
    {
        //search = "^(.+?) Immagine del profilo di";
        var reg = ParserStatic.ExecuteRegex(search);
        var k = lines.GetCapturingGroup(reg).DistinctNotEmpty();

        var mostFreqLines = ParserStatic.GetMostFreqLines(lines);
        k.AddRange(mostFreqLines);
        return k.Distinct().ToList();
    }


    // Identify and associate lines with respective speakers
    public IEnumerable<RigaDivisaPerPersone> IdentifySpeaker2(IEnumerable<string> lines)
    {
        if (Speakers == null)
            ExtractSpeakers(string.Join("\n", lines));

        lines = lines.Where(o => !string.IsNullOrWhiteSpace(o));
        Dictionary<string, List<string>> speakersDict = new Dictionary<string, List<string>>();
        string currentSpeaker = null;

        foreach (string curLine in lines)
        {
            if (IsSpeaker(curLine))
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
            }
        }

        foreach (var pair in speakersDict)
        {
            yield return new RigaDivisaPerPersone { Speaker = pair.Key, Messages = pair.Value };
        }
    }


    // Determine if a given line is a speaker's name
    public virtual bool IsSpeaker(string line)
    {
        return Speakers.Any(s => s.StartsWith(line));
    }


    // Extract a list of speakers from the given text
    public virtual void ExtractSpeakers(string text)
    {
        // Regex pattern to identify names or names with surnames
        Regex regex = new Regex(@"^(\w+)( \w+)?$", RegexOptions.Multiline);

        var matches = regex.Matches(text);

        // Extract matched speakers and return as a list
        Speakers = matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
    }
}
