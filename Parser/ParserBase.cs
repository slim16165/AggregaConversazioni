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
        var reg = ExecuteRegex(search);
        var k = lines.GetCapturingGroup(reg).DistinctNotEmpty();

        //Metodo alternativo, serco le righe ripetute più volte
        var mostFreqLines = GetMostFreqLines(lines);

        return k;
    }

    private static List<string> GetMostFreqLines(IEnumerable<string> lines)
    {
        return lines.GroupBy(s => s)
            .Select(group => new {
                Text = @group.Key,
                Count = @group.Count()
            })
            .Where(l => l.Count != 1
                        && !string.IsNullOrWhiteSpace(l.Text)
                        && l.Text.Length < 20)
            .OrderByDescending(x => x.Count)
            .Select(s => s.Text).ToList();
    }

    internal static Regex ExecuteRegex(string search)
    {
        return new Regex(search, RegexOptions.IgnoreCase | RegexOptions.Multiline);
    }

    protected static IEnumerable<RigaDivisaPerPersone> IdentifySpeaker2(IEnumerable<string> lines)
    {
        //Elimino le righe vuote
        lines = lines.Where(o => !string.IsNullOrWhiteSpace(o));

        var speakers2 = new[] { "Persona1", "Persona2", "Persona3" };
        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();


        var output = new Dictionary<string, Action<RigaDivisaPerPersone, string>>
        {
            { speakers2[0], (p, v) => p.Persona1 = v },
            { speakers2[1], (p, v) => p.Persona2 = v },
            { speakers2[2], (p, v) => p.Persona3 = v }
        };

        string currentBin = speakers2[0]; //Se non so chi sta parlando per primo scelgo a caso

        foreach (string curLine in lines)
        {
            //la riga corrente è il nome di una persona che parla
            if (speakers2.Contains(curLine))
            {
                //if (output.ContainsKey(currentBin))
                {
                    //output[currentBin].AppendText("\n\n");
                }
                currentBin = curLine;
            }
            else
            {
                if (!dict.ContainsKey(currentBin) || dict[currentBin] == null)
                    dict[currentBin] = new List<string>();
                else
                    dict[currentBin].Add(curLine);

                //output[currentBin].AppendText(curLine);

                var row = new RigaDivisaPerPersone();
                var function = output[currentBin];

                function(row, curLine);

                ////if(currentBin)
                //var k2 = new Func<RigaDivisaPerPersone, string, object>((p, v) => p.Persona1 = v);
                //var k3 = new Action<RigaDivisaPerPersone, string>((p, v) => p.Persona1 = v);

                yield return row;
            }
        }
    }

    protected internal static string ParseIo_LeiCiclico(string text)
    {
        //versione vecchia
        //Regex regexObj2 = new Regex(@"^(Io|Lei): ([^:]+?)[\n\s\r]+\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        Regex regexObj2 = new Regex(@"^(Io|Lei): ((?:(?!^(Io|Lei)).)+?)[\n\s\r]+\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        string prima = text, dopo;


        bool hasChanged;
        do
        {
            dopo = regexObj2.Replace(prima, @"$1: $2 ");
            hasChanged = prima != dopo;
            prima = dopo;
        } while (hasChanged);

        return dopo;
    }

    public static (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) ParseIo_LeiCiclico2(string text)
    {
        text = ParseIo_LeiCiclico(text);

        return (text, null, null);
    }
}