using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfApp1;

internal class Parser
{
    

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