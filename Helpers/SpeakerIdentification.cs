using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Helpers;
using AggregaConversazioni.Models;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Utils;

internal class SpeakerIdentification
{
    public static List<string> Speakers { get; set; }

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
    public static IEnumerable<RigaDivisaPerPersone> IdentifySpeaker2(IEnumerable<string> lines)
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
    public static bool IsSpeaker(string line)
    {
        return Speakers.Any(s => s.StartsWith(line));
    }

    // Extract a list of speakers from the given text
    public static void ExtractSpeakers(string text)
    {
        // Regex pattern to identify names or names with surnames
        Regex regex = new Regex(@"^(\w+)( \w+)?$", RegexOptions.Multiline);

        var matches = regex.Matches(text);

        // Extract matched speakers and return as a list
        Speakers = matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
    }
}