﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni
{
    class ParserEvernote : Parser
    {
        public static string AnalizzaEvernote(string text)
        {
            var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
            //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

            var lines2 = ApplyRegex(ref text, enumerable);

            return text;
        }

        public static string ApplyRegex(ref string text, IEnumerable<string> lines)
        {
            //Cancella fb groups
            List<(string from, string to)> regexes = new List<(string @from, string to)>()
            {
                //Se serve rimuovo gli spazi prima di ogni div PRIMA di rimuovere il div
                //oppure alla fine: ^\s+

                //Rimuovi link ai gruppi facebook
                (@"https://.{1,4}facebook\.com/groups/[^""<>\s]+", ""),
                //Rimuovi le classi css
                (@"\s*class=""[^""<>]+""", ""),
                //Rimuovi undefined
                (@"\bundefined\b", ""),
                //Evidenzia in giallo
                ("<span data-highlight=\"yellow\">(.+?)</span>", "<mark>$1</mark>"),
                //Elimina l'attributo dai tag semplici → ma serve?
                (@"<(\w+)[^<>]*>[^<>]*</\k<1>>", @"$0"),
                //Prende il contenuto dei tag ma cosa ci fa?
                (@"<a[^<>]*href=""([^""]+)""[^<>]*>\k<1></a>", "$1"),
                //Trova gli attributi dei tag candidati a essere rimossi (da usare su Regex Buddy)
                //(?!href|src)\b(\w+)="[^"]+",
                //Rimuovi i seguenti attributi dei tag (controllati e sicuri)
                (@"\b(charset|name|content|itemprop|rev|width|role|contenteditable|type|tabindex|markholder|value|fontsize)=""[^""]+""", ""),
                
                //Eliminazione span strani
                ("style=\"--[^\"]+\"", ""),
                ("style=\"font-family[^\"]+\"", ""),

                //Tag svuotati che non aggiungono nulla
                //per la ricerca in RegexBuddy - <(?!a|b|i|stong|mark)(\w+)[^<>]*>[^<>]*</\k<1>>
                //Tag span senza attributi
                (@"<(span)[^=""<>]*>(.*?)</\k<1>>", "$2"),
                
                
                //Tag che contengono degli a capo inutili subito dopo l'apertura
                (@"<(\w+)*>[\n|\r|\s]+(.*?)[\n|\r|\s]+</\k<1>>", "<$1>$2</$1>"),

                //Span vuoti
                (@"<(span)\s*>(.*?)</\k<1>>", "$2"),

                //Rimuovi tag inutili
                (@"^\s*</?(en-note|body|html|meta|input) *>[\s\n\r]*", ""),

                //Pulisci link di facebook (fbclid)
                (@"\??fbclid=\w+(?=\W)", ""),

                //Parsa i link 
                //("<a href=\"([^\"]+)\"[^<>]>(?=http)(.*?)</a>", "$1"), //quelli con anchor nuda
                (@"<a href=\""([^\""]+)\""[^<>]>(?=http)\k<1>(.*?)</a>", "$1"), //quelli con anchor nuda che si ripetono
                ("<a href=\"([^\"]+)\"[^<>]>(.*?)</a>", "[$1|$2]"),    //e quelli normali

                //Rimuovi link vuoti
                (@"<a href=""\s*>(.*?)</a>", ""),
                

                //Rimuovo i div
                ("</?div>", "\n"),
                //Rimuovo i troppi a capo
                (@"[\n|\r]+", @"\n"),

                //Rimuovi spazi inutili
                (@"^\s*<", "<"),

                //Trasforma gli HR in sezioni
                ("<hr/?>", "== Titolo sezione =="),


                //Correzione casini

                //Pulisco spazi dentro i tag e accapo subito dopo
                (@"<(\w+)\s+>\n", "<$1>"),
                //Pulisco gli accapo dopo alcuni tag
                
                //Pulisco i br multipli
                (@"<br>\n+", Environment.NewLine),


                //Elimina data-display- (che è rimasto così a caso dentro i tag div e negli span)
                (@"\bdata-display-\b", ""),
                (@"\bdata-\b", ""),

            };

            foreach (var reg in regexes)
            {
                text = Regex.Replace(text, reg.from, reg.to, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            }

            return text;
        }

        public override (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers) Parse(string text)
        {
            throw new NotImplementedException();
        }
    }
}
