using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Neo4j.Driver;

namespace AggregaConversazioni.Parser;

internal class SemanticLink
{
    public SemanticLink()
    {
    }

    public string Url { get; set; }
    public string Title { get; set; }
    public string ItalianTitle { get; set; }
    public string[] Tags { get; set; }
}

internal class MediaWiki2Neo4j
{

    public static void Prova()
    {
        var wikitextCollection = GetSampleWikitextCollection();

        NewMethod(wikitextCollection);
    }

    private static void NewMethod(List<string> wikitextCollection)
    {
        var semanticLinks = ParseTemplates(wikitextCollection);

        var queryStrings = ComposeTextQuery(semanticLinks);

        CallDb(queryStrings);
            
    }

    private static IEnumerable<SemanticLink> ParseTemplates(List<string> wikitextCollection)
    {
        foreach (var semanticLinkText in wikitextCollection)
        {
            yield return ParseTemplate(semanticLinkText);
        }
    }

    private static void CallDb(List<string> queryStrings)
    {
        var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));

        using (var session = driver.AsyncSession())
        {
            foreach (var queryString in queryStrings)
            {
                var result = session.RunAsync(queryString);
            }
        }
    }

    private static List<string> ComposeTextQuery(IEnumerable<SemanticLink> links)
    {
        List<string> queryStrings = new List<string>();
        foreach (SemanticLink semanticLink in links)
        {
            queryStrings.Add("CREATE (s:Sentence {url: $url, title: $title, italianTitle: $italianTitle})");
            var words = semanticLink.Title.Split(' ');
            foreach (var word in words)
            {
                queryStrings.Add($"CREATE (w:Word \\{{word: {word}\\}})");
                queryStrings.Add(
                    $"MATCH (w:Word), (s:Sentence) WHERE w.word = $word AND s.title = $title CREATE (w)-[r:CONTAINED_IN]->(s)");
            }
        }

        return queryStrings;
    }

    private static List<string> GetSampleWikitextCollection()
    {
        var wikitextCollection = new List<string>
        {
            "{{SemanticLink|url=https://www.youtube.com/watch?v=3rBiebA7ow4|title=Do Only Fans Models DESERVE High Value Men?|italianTitle=Le modelle di OnlyFans meritano gli uomini di alto valore?|tags=#redpill, #sessismo, #relazioni, #valore, #sessualità}}",
            "{{SemanticLink|url=https://www.youtube.com/watch?v=eLKuifN-Qrg|title=The REASON Why Women Do ONLYFANS Model|tags=#redpill}}",
            "{{SemanticLink|url=https://www.youtube.com/watch?v=zeZPqOPpdRI|title=The DELUSIONS Of ONLYFANS Models|tags=#redpill}}",
            "{{SemanticLink|url=https://www.youtube.com/watch?v=89rLYyfpu1M|title=Panel SPEAKS On ONLYFANS|tags=#redpill}}",
            "{{SemanticLink|url=https://www.youtube.com/watch?v=tRMNIOxfdKQ|title=Feminist Thought's On ONLYFANS|tags=#redpill}}",
            "{{SemanticLink|url=https://www.youtube.com/watch?v=Yquw31dV51k|title=Modern Woman BLAMES Men for Onlyfans|tags=#redpill}}"
        };
        return wikitextCollection;
    }

    private static SemanticLink ParseTemplate(string entry)
    {
        StringCollection resultList = new StringCollection();
        try
        {
            Regex regexObj = new Regex(@"{{SemanticLink\|url=(?<url>https?://[-A-Z0-9+&@#/%?=~_|$!:,.;]*[A-Z0-9+&@#/%=~_|$])\|title=(?<title>[^|]+?)\|tags=(?<tags>[^}]+?)}}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match matchResult = regexObj.Match(entry);
            while (matchResult.Success)
            {
                resultList.Add(matchResult.Groups["groupname"].Value);
                matchResult = matchResult.NextMatch();
            }
        }
        catch (ArgumentException ex)
        {
            // Syntax error in the regular expression
        }


        var properties = entry.Replace("{{SemanticLink|", "").Replace("}}", "").Split('|');
        var url = properties[0].Split('=')[1];
        var title = properties[1].Split('=')[1];
        var italianTitle = properties[2].Split('=')[1];
        var tags = properties[3].Split('=')[1].Split(',');

        var s = new SemanticLink();
        s.Url = url;
        s.Title = title;
        s.ItalianTitle = italianTitle;
        s.Tags = tags;

        return s;
    }
}