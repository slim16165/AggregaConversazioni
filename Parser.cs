using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

internal static class Parser
{
    public static (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) AnalizzaMessenger2(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

        var speakers = Parser.IdentifySpeakers(enumerable);

        var lines2 = ApplyRegex(ref text, enumerable);

        var k = IdentifySpeaker2(enumerable);
            
        return (text, k, speakers);
    }

    private static IEnumerable<RigaDivisaPerPersone> IdentifySpeaker2(IEnumerable<string> lines)
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

    public static IEnumerable<string> ApplyRegex(ref string text, IEnumerable<string> lines)
    {
        string shortSpeakerName;
        string longSpeakerName = "Stephanie Frogs";
        //shortSpeakerName = "Julia";
        longSpeakerName = "Julia Margini";
        longSpeakerName = "Francesco Arnaldi";
        longSpeakerName = "Sara Stefanile";
        shortSpeakerName = longSpeakerName.Split(' ').First();

        text = Regex.Replace(text, $"You unsent a message[\n\r]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+{longSpeakerName}[\n\r]+", "Lei: ",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, @"You sent[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, $"{shortSpeakerName} replied to you[\n\r]+", "Io: ",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, $"You replied to {shortSpeakerName}[\n\r]+", "Io: ",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        text = Regex.Replace(text, $"{longSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //elimino le ore 
        text = Regex.Replace(text, @"^\d{1,2}:\d{2} [ap]m[\n\r]", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        Regex regexObj2 = new Regex(@"^(Io|Lei): (.*)\n\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        for (int i = 0; i < 10; i++)
        {
            text = regexObj2.Replace(text, @"$1: $2 ");
        }

        return lines;
    }

    public static List<string> IdentifySpeakers(IEnumerable<string> lines)
    {
        //^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)
            
        //Cerco quelli con replied to you
        Regex regexObj = new Regex("^(.+?) replied to you", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var speakers = lines.Select( subjectString => regexObj.Match(subjectString).Groups[1].Value).Where(s=> !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

        //Cerco le righe ripetute più volte
        var mostFreqLines = lines.GroupBy(s => s)
            .Select(group => new {
                Text = @group.Key,
                Count = @group.Count()
            })
            .Where(l => l.Count != 1 
                        && !string.IsNullOrWhiteSpace(l.Text)
                        && l.Text.Length < 20)
            .OrderByDescending(x => x.Count).ToList();

        return mostFreqLines.Select(s =>s.Text).ToList();
    }
}