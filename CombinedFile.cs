### Contenuto di App.xaml ###
<Application x:Class="AggregaConversazioni.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>

        <ZoomPercentageConverter x:Key="BoolToVisibilityConverter" />
    </Application.Resources>
</Application>


### Contenuto di App.xaml.cs ###
using System.Windows;

namespace AggregaConversazioni;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
}


### Contenuto di ConversionTypeItem.cs ###
using AggregaConversazioni.Transformers;

namespace AggregaConversazioni;

public class ConversionTypeItem
{
    public string DisplayName { get; set; }
    public TransformerType Type { get; set; }
}


### Contenuto di DebugHelper.cs ###
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;
using ConsoleTableExt;

namespace AggregaConversazioni.Helpers
{
    /// <summary>
    /// Provides helper methods for debugging with regex patterns.
    /// </summary>
    internal static class DebugHelper
    {
        /// <summary>
        /// Annotates a text with given regex patterns and their replacement values.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="regexes">A list of regex patterns and their corresponding replacements.</param>
        /// <returns>The annotated text.</returns>
        public static string Annotate(string text, List<RegexDescription> regexes)
        {
            var res = Annotate(text, regexes.Select(r => (r.From, r.To)).ToList());
            return res;
        }

        /// <summary>
        /// Annotates a text with given source strings and their replacement values.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="regexes">A list of source strings and their corresponding replacements.</param>
        /// <returns>The annotated text.</returns>
        public static string Annotate(string text, List<(string from, string to)> regexes)
        {
            // Calcola e restituisce la stringa annotata
            return CreateAnnotatedString(text, regexes);
        }

        // Il metodo PrepareAnnotationDataForUI resta disponibile per essere usato
        // dal ViewModel se necessario, ad esempio:
        public static ObservableCollection<RegexDebugData> GetAnnotationData(string text, List<(string from, string to)> regexes)
        {
            return PrepareAnnotationDataForUI(text, regexes);
        }


        public static string CreateAnnotatedString(string text, List<(string from, string to)> replacementRules)
        {
            //Console.WriteLine("| from | to | sostituzioni |");
            //Console.WriteLine("|------|----|--------------|");
            //var enumerable = sostituzioni.Select(s => $"| {s.Source} | {s.Result} |");

            var tableData = replacementRules.Select(rule =>
            {
                var data = CalculateReplacement(text, rule);
                return new List<object>
                {
                    Escape(rule.from),
                    Escape(rule.to),
                    data.count,
                    data.countDist
                };
            }).ToList();

            var tableRender = ConsoleTableBuilder
                .From(tableData)
                .WithColumn("Regex from", "Regex to")
                .WithPaddingLeft(string.Empty)
                .WithFormat(ConsoleTableBuilderFormat.Default)
                .Export();

            return tableRender.ToString();
        }

        public static ObservableCollection<RegexDebugData> PrepareAnnotationDataForUI(string text, List<(string from, string to)> regexes)
        {
            var tableDataViewModel = (
                from reg in regexes
                let replacementData = CalculateReplacement(text, reg)
                select new RegexDebugData
                {
                    From = Escape(reg.@from),
                    To = Escape(reg.to),
                    Count = replacementData.count,
                    CountDistinct = replacementData.countDist,
                }).ToList();

            return new ObservableCollection<RegexDebugData>(tableDataViewModel);
        }


        /// <summary>
        /// Calculates the replacement operations on a given text for a specific regex pattern.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <param name="reg">The regex pattern and its replacement.</param>
        /// <returns>The details of the replacement operations.</returns>
        private static (string replacementDetails, int count, int countDist) CalculateReplacement(string text, (string from, string to) reg)
        {
            // 1. Trova tutte le corrispondenze del pattern regex nel testo fornito.
            var matches = Regex.Matches(text, reg.from, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

            // 2. Per ogni corrispondenza trovata, crea un nuovo oggetto anonimo che contiene la stringa originale 
            // e il risultato previsto della regex replace
            var executedReplacements = (from Match match in matches
                select new
                {
                    Source = match.Value,
                    Result = match.Result(reg.to)
                }).ToList();

            // 3. Conta il numero totale di corrispondenze (e sostituzioni previste) trovate.
            var count = executedReplacements.Count;

            // 4. Filtra l'elenco di tutte le sostituzioni previste per rimuovere le duplicazioni e conta le sostituzioni uniche.
            var distinctExecutedReplacements = executedReplacements.Distinct().ToList();
            var countDist = distinctExecutedReplacements.Count;

            // 5. Crea una rappresentazione dettagliata delle sostituzioni. Se ci sono sostituzioni uniche da effettuare, 
            // combina tutte queste sostituzioni in una stringa. La stringa avrà un formato in cui ogni sostituzione 
            // originale e il suo risultato sono separati da una freccia e ogni coppia è separata da un ritorno a capo.
            var replacementDetails = distinctExecutedReplacements.Any()
                ? distinctExecutedReplacements.Select(s => $"{Escape(s.Source)} → {Escape(s.Result)}")
                    .Aggregate((a, b) => a + "\r" + b)
                : "";

            // 6. Ritorna i dettagli delle sostituzioni, il conteggio totale e il conteggio distinto.
            return (replacementDetails, count, countDist);

        }

        /// <summary>
        /// Escapes specific characters in a given string.
        /// </summary>
        private static string Escape(string source)
        {
            //source = source.Replace("\n", "");
            //source = source.Replace("\r", "");
            return source; //.Substring(0, Math.Min(source.Length, 30));
        }
    }
}


### Contenuto di Extension.cs ###
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


### Contenuto di MediaWiki2Neo4j.cs ###
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Neo4j.Driver;

namespace AggregaConversazioni.Integrations;

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


### Contenuto di MainWindow.xaml ###
<Window x:Class="AggregaConversazioni.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:telerik="http://schemas.telerik.com/2008/xaml/presentation"
        mc:Ignorable="d"
        Title="Text Converter Pro" 
        Height="800" Width="1200"
        WindowStartupLocation="CenterScreen"
        MinHeight="600" MinWidth="800">

    <DockPanel LastChildFill="False">
        <!-- Toolbar fissa in alto -->
        <ToolBar DockPanel.Dock="Top" Padding="5" Background="#EEE">
            <!-- Selezione Tipo di Conversione -->
            <StackPanel Orientation="Horizontal">
                <Label Content="Tipo di Conversione:" VerticalAlignment="Center"/>
                <ComboBox x:Name="ConversionTypeComboBox"
                          Width="200"
                          ToolTip="Seleziona il tipo di conversione"
                          ItemsSource="{Binding ConversionTypes}"
                          DisplayMemberPath="DisplayName"
                          SelectedItem="{Binding SelectedConversionType}"
                          Margin="5,0,0,0"/>
            </StackPanel>
            <Separator/>
            <!-- Bottone Converti con icona FontAwesome -->
            <Button Command="{Binding ConvertCommand}" Width="100" ToolTip="Avvia la conversione">
                <StackPanel Orientation="Horizontal">
                    <TextBlock FontFamily="FontAwesome" 
                               Text="&#xf1ec;" 
                               FontSize="16" 
                               VerticalAlignment="Center"/>
                    <TextBlock Text="Converti" 
                               Margin="5,0,0,0" 
                               VerticalAlignment="Center"/>
                </StackPanel>
            </Button>
            <!-- Bottone Carica Esempio con icona FontAwesome -->
            <Button Command="{Binding LoadSampleCommand}" Width="120" ToolTip="Carica un esempio" Margin="5,0,0,0">
                <StackPanel Orientation="Horizontal">
                    <TextBlock FontFamily="FontAwesome" 
                               Text="&#xf040;" 
                               FontSize="16" 
                               VerticalAlignment="Center"/>
                    <TextBlock Text="Carica Esempio" 
                               Margin="5,0,0,0" 
                               VerticalAlignment="Center"/>
                </StackPanel>
            </Button>
            <!-- Toggle per il Debug Mode -->
            <ToggleButton x:Name="DebugToggle"
                          Content="Debug Mode"
                          IsChecked="{Binding ShowDebug}"
                          ToolTip="Abilita/disabilita la visualizzazione dei dati di debug"
                          Margin="10,0,0,0"/>
        </ToolBar>

        <!-- Layout principale: Grid con GridSplitter e pannello Debug racchiuso in un Expander -->
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="2*"/>
                <ColumnDefinition Width="5"/>
                <ColumnDefinition Width="3*"/>
            </Grid.ColumnDefinitions>

            <!-- Area Input/Output in TabControl (prima colonna) -->
            <TabControl Grid.Column="0">
                <TabItem Header="Input">
                    <TextBox x:Name="InputTextBox"
                             AcceptsReturn="True"
                             TextWrapping="Wrap"
                             FontFamily="Consolas"
                             Padding="5"
                             VerticalScrollBarVisibility="Auto"
                             Text="{Binding InputText, UpdateSourceTrigger=PropertyChanged}"/>
                </TabItem>
                <TabItem Header="Output">
                    <TextBox x:Name="OutputTextBox"
                             IsReadOnly="True"
                             TextWrapping="Wrap"
                             FontFamily="Consolas"
                             Padding="5"
                             VerticalScrollBarVisibility="Auto"
                             Text="{Binding OutputText}"/>
                </TabItem>
            </TabControl>

            <!-- GridSplitter per ridimensionare le due aree -->
            <GridSplitter Grid.Column="1"
                          HorizontalAlignment="Stretch"
                          VerticalAlignment="Stretch"
                          Width="5"
                          Background="Gray"
                          ShowsPreview="True"
                          Cursor="SizeWE"/>

            <!-- Pannello Debug in un Expander (terza colonna) -->
            <Expander Grid.Column="2" Header="Debug" IsExpanded="True"
                      Visibility="{Binding ShowDebug, Converter={StaticResource BoolToVisibilityConverter}}">
                <DockPanel>
                    <TabControl DockPanel.Dock="Top">
                        <TabItem Header="Regole Applicate">
                            <telerik:RadGridView x:Name="RulesGrid"
                                                 ItemsSource="{Binding AppliedRules}"
                                                 AutoGenerateColumns="False"
                                                 IsReadOnly="True">
                                <telerik:RadGridView.Columns>
                                    <telerik:GridViewDataColumn Header="Pattern" DataMemberBinding="{Binding From}"/>
                                    <telerik:GridViewDataColumn Header="Sostituzione" DataMemberBinding="{Binding To}"/>
                                    <telerik:GridViewDataColumn Header="Occorrenze" DataMemberBinding="{Binding Count}"/>
                                </telerik:RadGridView.Columns>
                            </telerik:RadGridView>
                        </TabItem>
                        <TabItem Header="Speaker">
                            <ListBox x:Name="SpeakersList"
                                     ItemsSource="{Binding Speakers}"
                                     ScrollViewer.VerticalScrollBarVisibility="Auto"/>
                        </TabItem>
                    </TabControl>
                    <!-- Barra di stato interna al pannello Debug -->
                    <StatusBar DockPanel.Dock="Bottom" Margin="0,5,0,0">
                        <TextBlock Text="{Binding ConversionStats}"/>
                        <ProgressBar Width="200" Margin="10,0,0,0"
                                     Value="{Binding ConversionProgress}"
                                     Visibility="{Binding IsConverting, Converter={StaticResource BoolToVisibilityConverter}}"/>
                    </StatusBar>
                </DockPanel>
            </Expander>
        </Grid>

        <!-- Status bar generale in basso -->
        <StatusBar DockPanel.Dock="Bottom">
            <TextBlock Text="Pronto"/>
            <Separator/>
            <TextBlock Text="{Binding InputTextLength}"/>
            <TextBlock Text="caratteri"/>
            <Separator/>
            <TextBlock Text="{Binding OutputTextLength}"/>
            <TextBlock Text="caratteri convertiti"/>
        </StatusBar>
    </DockPanel>
</Window>


### Contenuto di MainWindow.xaml.cs ###
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AggregaConversazioni.Models;
using AggregaConversazioni.Transformers;

namespace AggregaConversazioni
{
    // Classe per rappresentare il tipo di conversione

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Proprietà per il binding
        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
                OnPropertyChanged(nameof(InputTextLength));
            }
        }

        private string _outputText;
        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
                OnPropertyChanged(nameof(OutputTextLength));
            }
        }

        public int InputTextLength => string.IsNullOrEmpty(InputText) ? 0 : InputText.Length;
        public int OutputTextLength => string.IsNullOrEmpty(OutputText) ? 0 : OutputText.Length;

        private string _conversionStats;
        public string ConversionStats
        {
            get => _conversionStats;
            set
            {
                _conversionStats = value;
                OnPropertyChanged(nameof(ConversionStats));
            }
        }

        private double _conversionProgress;
        public double ConversionProgress
        {
            get => _conversionProgress;
            set
            {
                _conversionProgress = value;
                OnPropertyChanged(nameof(ConversionProgress));
            }
        }

        private bool _showDebug;
        public bool ShowDebug
        {
            get => _showDebug;
            set
            {
                _showDebug = value;
                OnPropertyChanged(nameof(ShowDebug));
            }
        }

        public ObservableCollection<ConversionTypeItem> ConversionTypes { get; set; }
        private ConversionTypeItem _selectedConversionType;
        public ConversionTypeItem SelectedConversionType
        {
            get => _selectedConversionType;
            set
            {
                _selectedConversionType = value;
                OnPropertyChanged(nameof(SelectedConversionType));
            }
        }

        // Collezioni per Debug e Speaker (se necessarie)
        public ObservableCollection<RegexDebugData> AppliedRules { get; set; }
        public ObservableCollection<string> Speakers { get; set; }

        // Comandi per le azioni
        public ICommand ConvertCommand { get; set; }
        public ICommand LoadSampleCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Inizializza le collezioni
            ConversionTypes = new ObservableCollection<ConversionTypeItem>
            {
                new ConversionTypeItem { DisplayName = "Facebook", Type = TransformerType.Facebook },
                new ConversionTypeItem { DisplayName = "Messenger", Type = TransformerType.Messenger },
                new ConversionTypeItem { DisplayName = "Instagram", Type = TransformerType.Instagram },
                new ConversionTypeItem { DisplayName = "Telegram", Type = TransformerType.Telegram },
                new ConversionTypeItem { DisplayName = "Evernote", Type = TransformerType.Evernote },
                new ConversionTypeItem { DisplayName = "Io/Lei ciclico", Type = TransformerType.IoLeiCiclico },
                new ConversionTypeItem { DisplayName = "Markdown to Wiki", Type = TransformerType.MarkdownToWiki }
            };
            SelectedConversionType = ConversionTypes[0];

            AppliedRules = new ObservableCollection<RegexDebugData>();
            Speakers = new ObservableCollection<string>();

            // Inizializza i comandi
            ConvertCommand = new RelayCommand(o => ConvertText(), o => !string.IsNullOrWhiteSpace(InputText));
            LoadSampleCommand = new RelayCommand(o => LoadSampleText());

            // Stato iniziale
            InputText = "";
            OutputText = "";
            ConversionStats = "Pronto";
            ConversionProgress = 0;
            ShowDebug = false;
        }

        private void ConvertText()
        {
            try
            {
                ConversionProgress = 0;
                ConversionStats = "Conversione in corso...";

                // Crea il transformer in base al tipo selezionato
                var transformer = TransformerFactory.Create(SelectedConversionType.Type);
                // Esegue la trasformazione
                OutputText = transformer.Transform(InputText);

                ConversionStats = "Conversione completata.";
                ConversionProgress = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nella conversione: {ex.Message}");
                ConversionStats = "Errore durante la conversione.";
            }
        }

        private void LoadSampleText()
        {
            // Esempio per MarkdownToWiki oppure testo generico per altri tipi
            if (SelectedConversionType.Type == TransformerType.MarkdownToWiki)
            {
                InputText = "# Titolo Principale\r\nTesto **in grassetto** e *in corsivo*.\r\n- Elemento 1\r\n- Elemento 2\r\n[Link](http://example.com)";
            }
            else
            {
                InputText = $"Esempio di testo per {SelectedConversionType.DisplayName}";
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Implementazione semplice di ICommand
}


### Contenuto di RegexDebugData.cs ###
namespace AggregaConversazioni.Models;

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


### Contenuto di RegexDescription.cs ###
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Models;

public enum RegexCategory
{
    GeneralCleanup,
    SpeakerYouReplacement,
    SpeakerOtherReplacement,
    ReplyBasedReplacement
}

public class RegexDescription
{
    public Regex Regex { get; private set; }
    public string Description { get; set; }
    public string To { get; set; }
    public string From => Regex.ToString();
    public RegexCategory Category { get; set; } // Added categorization
    public bool IsOrderImportant { get; set; }  // A flag to check if the order of application is important

    public RegexDescription((string from, string to) regex, string description = "", RegexCategory category = RegexCategory.GeneralCleanup, bool isOrderImportant = false)
    {
        Regex = new Regex(regex.from);
        Description = description;
        To = regex.to;
        Category = category;
        IsOrderImportant = isOrderImportant;
    }

    public string Replace(string input)
    {
        return Regex.Replace(input, To);
    }
}

public class RegexGroup
{
    public List<RegexDescription> RegexRules { get; set; } = new List<RegexDescription>();
    public bool IsOrderImportant { get; set; } // Indica se l'ordine delle regex nel gruppo è importante
    public string GroupName { get; set; } // Una descrizione o un nome per il gruppo, opzionale

    public RegexGroup(string groupName, bool isOrderImportant = false)
    {
        GroupName = groupName;
        IsOrderImportant = isOrderImportant;
    }

    // Potresti anche avere un metodo per applicare tutte le regex del gruppo a una stringa di input
    public string ApplyAll(string input)
    {
        foreach (var rule in RegexRules)
        {
            input = rule.Replace(input);
        }
        return input;
    }
}


### Contenuto di RigaDivisaPerPersone.cs ###
using System.Collections.Generic;

namespace AggregaConversazioni.Models;

public class RigaDivisaPerPersone
{
    public Dictionary<string, string> SpeakersMessages { get; set; } = new Dictionary<string, string>();

    // Puoi aggiungere altri metodi o propriet� se necessario.

    public string Speaker { get; set; }
    public List<string> Messages { get; set; } = new List<string>();
}


### Contenuto di ITextTransformer.cs ###
namespace AggregaConversazioni.Parsers;

public interface ITextTransformer
{
    string Transform(string input);
}


### Contenuto di ParserBase.cs ###
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Helpers;
using AggregaConversazioni.Models;

namespace AggregaConversazioni.Parsers;

public abstract class ParserBase : ITextTransformer
{
    protected ParserContext Context { get; } = new ParserContext();

    // Table for debugging purposes
    public static List<string> Speakers { get; set; }


    // Abstract method to parse the text and return text, a set of lines, and identified speakers
    public abstract (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text);
    
    public ParserBase WithText(string originalText)
    {
        Context.OriginalText = originalText;
        return this;
    }

    public ParserBase SplitTextIntoLines()
    {
        Context.TextLines = Regex.Split(Context.OriginalText, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        return this;
    }

    public (string, IEnumerable<RigaDivisaPerPersone>, List<string>) GetResult()
    {
        // TODO: Return appropriate values based on the Context
        return (Context.OriginalText, null, null); // Temporary return value, adjust as necessary
    }

    public abstract ParserBase IdentifySpeakers();
    public abstract ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null);

    public ParserBase ApplyRegexAndClean()
    {
        ApplyAllPatterns();
        ApplyPatternsWithNewLines();
        AnnotateDebugOutput();
        ApplyPostProcessingPatterns();

        return this;
    }

    protected virtual void ApplyAllPatterns()
    {
        foreach (var regexPattern in Context.RegexGroups)
        {
            Context.OriginalText = regexPattern.ApplyAll(Context.OriginalText);
        }
    }

    protected virtual void ApplyPatternsWithNewLines()
    {
        var regexReplacements = Context.RegexGroups.SelectMany(group => group.RegexRules).ToList();
        regexReplacements = RegexReplacementsWithNewLine(regexReplacements);

        foreach (var pattern in regexReplacements)
        {
            Context.OriginalText = Regex.Replace(Context.OriginalText, pattern.From + "\n?", pattern.To,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }
    }

    public List<RegexDescription> RegexReplacementsWithNewLine(List<RegexDescription> regexReplacements)
    {
        var regexWithPotentialNewLine = regexReplacements.Select(pattern =>
            new RegexDescription((pattern.From + Environment.NewLine, pattern.To), pattern.Description));

        regexReplacements = regexWithPotentialNewLine.Union(regexReplacements).ToList();
        return regexReplacements;
    }

    protected virtual void AnnotateDebugOutput()
    {
        var regexReplacements = Context.RegexGroups.SelectMany(group => group.RegexRules).ToList();
        Context.DebugOutputTable = DebugHelper.Annotate(Context.OriginalText, regexReplacements);
    }

    protected virtual void ApplyPostProcessingPatterns()
    {
        //...
    }

    public string Transform(string input)
    {
        throw new NotImplementedException();
    }
}


### Contenuto di ParserEvernote.cs ###
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;

namespace AggregaConversazioni.Parsers;

class ParserEvernote : ParserBase
{
    public static string AnalizzaEvernote(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

        var lines2 = ApplyRegex(ref text, enumerable);

        return text;
    }

    private static string ApplyRegex(ref string text, IEnumerable<string> lines)
    {
        //Rimuovere link e informazioni inutili dall'HTML
        List<(string from, string to)> regexesHtml = new List<(string @from, string to)>()
        {
            //Rimuovi link ai gruppi facebook
            (@"https://.{1,4}facebook\.com/groups/[^""<>\s]+", ""),
                
            //Rimuovi le classi css
            (@"\s*class=""[^""<>]+""", ""),
                
            //Elimina l'attributo dai tag semplici → ma serve?
            (@"<(\w+)[^<>]*>[^<>]*</\k<1>>", @"$0"),
                
            //Prende il contenuto dei tag ma cosa ci fa?
            (@"<a[^<>]*href=""([^""]+)""[^<>]*>\k<1></a>", "$1"),
                
            //Rimuovi undefined
            (@"\bundefined\b", ""),
                
            //Eliminazione span strani
            ("style=\"--[^\"]+\"", ""),
            ("style=\"font-family[^\"]+\"", ""),
                
            //Span vuoti
            (@"<(span)\s*>(.*?)</\k<1>>", "$2"),

            //Rimuovi tag inutili
            (@"^\s*</?(en-note|body|html|meta|input) *>[\s\n\r]*", ""),

            //Pulisci link di facebook (fbclid)
            (@"\??fbclid=\w+(?=\W)", ""),

            //Tag che contengono degli a capo inutili subito dopo l'apertura
            (@"<(\w+)*>[\n|\r|\s]+(.*?)[\n|\r|\s]+</\k<1>>", "<$1>$2</$1>"),

            //Rimuovo i div
            ("</?div>", "\n"),

            //Rimuovi spazi inutili
            (@"^\s*<", "<"),

            //Correzione casini

            //Rimuovi link vuoti
            (@"<a href=""\s*>(.*?)</a>", ""),

            //Pulisco spazi dentro i tag e accapo subito dopo
            (@"<(\w+)\s+>\n", "<$1>"),
            //Pulisco gli accapo dopo alcuni tag
                
            //Pulisco i br multipli
            (@"<br>\n+", Environment.NewLine),


            //Elimina data-display- (che è rimasto così a caso dentro i tag div e negli span)
            (@"\bdata-display-\b", ""),
            (@"\bdata-\b", ""),
        };


        List<(string from, string to)> helperRegexes = new List<(string @from, string to)>()
        {
            //Tag svuotati che non aggiungono nulla
            //per la ricerca in RegexBuddy - <(?!a|b|i|stong|mark)(\w+)[^<>]*>[^<>]*</\k<1>>
            //Tag span senza attributi
            (@"<(span)[^=""<>]*>(.*?)</\k<1>>", "$2"),

            //Evidenzia in giallo
            ("<span data-highlight=\"yellow\">(.+?)</span>", "<mark>$1</mark>"),

            //Trova gli attributi dei tag candidati a essere rimossi (da usare su Regex Buddy)
            //(?!href|src)\b(\w+)="[^"]+",
            //Rimuovi i seguenti attributi dei tag (controllati e sicuri)
            (@"\b(charset|name|content|itemprop|rev|width|role|contenteditable|type|tabindex|markholder|value|fontsize)=""[^""]+""",
                ""),
        };

        List<(string from, string to)> mediawikiRegexes = new List<(string @from, string to)>()
        {
            //Parsa i link 
            //("<a href=\"([^\"]+)\"[^<>]>(?=http)(.*?)</a>", "$1"), //quelli con anchor nuda
            (@"<a href=\""([^\""]+)\""[^<>]>(?=http)\k<1>(.*?)</a>", "$1"), //quelli con anchor nuda che si ripetono
            ("<a href=\"([^\"]+)\"[^<>]>(.*?)</a>", "[$1|$2]"), //e quelli normali

            //Trasforma gli HR in sezioni
            ("<hr/?>", "== Titolo sezione =="),

        };

        //Cancella fb groups
        List <(string from, string to)> genericRegexes = new List<(string @from, string to)>()
        {
            //Se serve rimuovo gli spazi prima di ogni div PRIMA di rimuovere il div
            //oppure alla fine: ^\s+


            //Rimuovo i troppi a capo
            (@"[\n|\r]+", @"\n"),
        };

        foreach (var (from, to) in genericRegexes)
        {
            text = Regex.Replace(text, from, to, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        return text;
    }

    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        throw new NotImplementedException();
    }

    public override ParserBase IdentifySpeakers()
    {
        throw new NotImplementedException();
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        throw new NotImplementedException();
    }
}


### Contenuto di ParserInstagram.cs ###
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;
using AggregaConversazioni.Utils;

namespace AggregaConversazioni.Parsers;

class ParserInstagram : ParserBase
{
    protected static string ApplyRegex(ref string text, string speaker)
    {
        List<(string from, string to)> regexes = new List<(string @from, string to)>()
        {
            // regexGiornoEOra =
            (@"(Lunedì|Martedì|Mercoledì|Giovedì|Sabato|Domenica) \d{1,2}:\d{2}", ""),
            //regexUsernameInterloc
            ("^Immagine del profilo di (.+?)$", "Lei: "),
            //Elimino le ore
            (@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""),
            (@"Annulla l'invio del messaggio", ""),
            (@"Mi piace", ""),
            (@"Copia", ""),
            (@"❤️", ""),
            //
            //@"^(Io|Lei): (.*)\n\k<1>: ", ""
        };

        string text1 = text;
        foreach (var (from, to) in regexes)
        {
            text1 = Regex.Replace(text1, from + "\n?", to,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        text = text1;

        return text;
    }


    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();
        //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)

        //Cerco quelli con Immagine del profilo di 
        string search = "^Immagine del profilo di (.+?)$";
        var speakers = SpeakerIdentification.IdentifySpeakersBySearchString(enumerable, search);
        var speaker = speakers.Single();

        var lines2 = ApplyRegex(ref text, speaker);

        var k = SpeakerIdentification.IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }

    public override ParserBase IdentifySpeakers()
    {
        throw new System.NotImplementedException();
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        throw new System.NotImplementedException();
    }
}


### Contenuto di ParserIoLeiCiclico.cs ###
using System.Collections.Generic;
using AggregaConversazioni.Models;

namespace AggregaConversazioni.Parsers;

public class ParserIoLeiCiclico : ParserBase
{
    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        text = ParserStatic.ParseIo_LeiCiclico(text);
        return (text, null, null);
    }

    public override ParserBase IdentifySpeakers()
    {
        return this;
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        return this;
    }
}


### Contenuto di ParserMessenger.cs ###
using System.Collections.Generic;
using System.Linq;
using AggregaConversazioni.Models;
using AggregaConversazioni.Utils;

namespace AggregaConversazioni.Parsers;

public class ParserContext
{
    public string OriginalText { get; set; }
    public IEnumerable<string> TextLines { get; set; }
    public List<string> IdentifiedSpeakers { get; set; }
    public List<RegexGroup> RegexGroups { get; set; } = new List<RegexGroup>();
    public string DebugOutputTable { get; set; }
}

internal class ParserMessenger : ParserBase
{
    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string originalText)
    {
        var result = new ParserMessenger()
            .WithText(originalText)
            .SplitTextIntoLines()
            .IdentifySpeakers()
            .InitializeRegexPatterns("Laura Eileen Gallo")
            .ApplyRegexAndClean()
            .GetResult();

        //// Process the original text based on certain regex patterns
        //var processedLines = ApplyRegexAndClean(ref originalText, textLines, regexGroups);

        return result; //(originalText, speakersFromMethod2, speakersFromPattern);
    }

    protected override void ApplyPostProcessingPatterns()
    {
        Context.OriginalText = ParserStatic.ParseIo_LeiCiclico(Context.OriginalText);
    }

    public override ParserBase IdentifySpeakers()
    {
        // Search for lines containing "Immagine del profilo di" to identify potential speakers
        string profileImagePattern = "^(.+?) Immagine del profilo di";
        var speakersFromPattern = SpeakerIdentification.IdentifySpeakersBySearchString(Context.TextLines, profileImagePattern);

        // Add the identified speakers to the Context or use them however you need
        Context.IdentifiedSpeakers = speakersFromPattern;

        return this;
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        if (shortSpeakerName == null)
            shortSpeakerName = fullSpeakerName.Split(' ').First();

        // Set of regex patterns and their replacements with corresponding description
        var messageOperations = new RegexGroup("Message Operations")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"You unsent a message", ""), "Eliminate the literal message: 'You unsent a message'")
            }
        };

        var speakerNameReplacements = new RegexGroup("Speaker Name Replacements", true) // True indica che l'ordine è importante
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"{shortSpeakerName}{fullSpeakerName}", "Lei: "), "Replace short and full speaker names with 'Lei: '"),
                new(($"You sent", "Io: "), "Replace 'You sent' with 'Io: '"),
                new(($"{shortSpeakerName}", "Lei: "), $"Replace '{shortSpeakerName}' with 'Lei: '"),
                new(($"{fullSpeakerName}", "Lei: "), $"Replace '{fullSpeakerName}' with 'Lei: '"),
            }
        };

        var replyOperations = new RegexGroup("Reply Operations")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"{shortSpeakerName} replied to you", "Io: "), $"Replace '{shortSpeakerName} replied to you' with 'Io: '"),
                new(($"You replied to {shortSpeakerName}", "Io: "), $"Replace 'You replied to {shortSpeakerName}' with 'Io: '"),
                new(($"{shortSpeakerName} replied to themself", "Lei: "), $"Replace '{shortSpeakerName} replied to themself' with 'Lei: '")
            }
        };

        var appSpecificOperations = new RegexGroup("App Specific Operations - Noted")
        {
            RegexRules = new List<RegexDescription>
            {
                new(($"^Enter", ""), "Eliminate 'Enter' from the start of lines"),
                new((@"^\d{1,2}:\d{2} [ap]m[\n\r]", ""), "Eliminate time-stamp hours from the start of lines")
            }
        };

        List<RegexGroup> regexGroups = new List<RegexGroup>
        {
            messageOperations,
            speakerNameReplacements,
            replyOperations,
            appSpecificOperations
        };

        Context.RegexGroups = regexGroups;

        return this;
    }
}


### Contenuto di ParserStatic.cs ###
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AggregaConversazioni.Parsers
{
    internal static class ParserStatic
    {
        public static List<string> GetMostFreqLines(IEnumerable<string> lines)
        {
            return lines.GroupBy(s => s)
                .Select(group => new {
                    Text = group.Key,
                    Count = group.Count()
                })
                .Where(l => l.Count != 1
                            && !string.IsNullOrWhiteSpace(l.Text)
                            && l.Text.Length < 20)
                .OrderByDescending(x => x.Count)
                .Select(s => s.Text).ToList();
        }

        public static string ParseIo_LeiCiclico(string text)
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

        internal static Regex ExecuteRegex(string search)
        {
            return new Regex(search, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }
}


### Contenuto di ParserTelegram.cs ###
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AggregaConversazioni.Models;
using AggregaConversazioni.Utils;

namespace AggregaConversazioni.Parsers;

class ParserTelegram : ParserBase
{
    private static string ApplyRegex(ref string text, IEnumerable<string> lines)
    {
        var longSpeakerName = "Sara Stefanile";
        var shortSpeakerName = longSpeakerName.Split(' ').First();

        List<(string from, string to)> regexes = new List<(string @from, string to)>()
        {
            //Sara Pinca, [16/01/2022 01:08] → '''Sara Pinca'''
            (@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
            //(@"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n", "'''$1: '''")
        };

        string text1 = text;
        foreach (var reg in regexes)
        {
            text1 = Regex.Replace(text1, reg.from + "\n?", reg.to,
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        }

        text = text1;

        return text;

    }

    public override (string parsedText, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> identifiedSpeakers) Parse(string text)
    {
        var enumerable = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim()).ToList();

        //Cerco quelli con Immagine del profilo di 
        string searchWithCapturingGroup = @"^(.+?), \[\d{2}/\d{2}/\d{4} \d{2}:\d{2}\]$\n";
        var speakers = SpeakerIdentification.IdentifySpeakersBySearchString(enumerable, searchWithCapturingGroup);

        var lines2 = ApplyRegex(ref text, enumerable);

        var k = SpeakerIdentification.IdentifySpeaker2(enumerable);

        return (text, k, speakers);
    }

    public override ParserBase IdentifySpeakers()
    {
        throw new System.NotImplementedException();
    }

    public override ParserBase InitializeRegexPatterns(string fullSpeakerName, string shortSpeakerName = null)
    {
        throw new System.NotImplementedException();
    }
}


### Contenuto di RelayCommand.cs ###
using System;
using System.Windows.Input;

namespace AggregaConversazioni;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    public void Execute(object parameter) => _execute(parameter);

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}


### Contenuto di MarkdownToWikiConverter.cs ###
namespace AggregaConversazioni.Transformers;

public class MarkdownToWikiConverter : RegexBasedTransformer
{
    public MarkdownToWikiConverter()
    {
        // Converti titoli di livello 1: "# Titolo" → "== Titolo =="
        RegexReplacements.Add((@"^# (.+)$", "== $1 =="));

        // Converti titoli di livello 2: "## Titolo" → "=== Titolo ==="
        RegexReplacements.Add((@"^## (.+)$", "=== $1 ==="));

        // Converti il grassetto: **testo** → '''testo'''
        RegexReplacements.Add((@"\*\*(.+?)\*\*", "'''$1'''"));

        // Converti il corsivo: *testo* → ''testo''
        RegexReplacements.Add((@"\*(.+?)\*", "''$1''"));

        // Converti i link Markdown: [Testo](url) → [[url|Testo]]
        RegexReplacements.Add((@"\[(.+?)\]\((.+?)\)", "[[$2|$1]]"));

        // Converti le liste: "- Elemento" → "* Elemento"
        RegexReplacements.Add((@"^- (.+)$", "* $1"));
    }
}


### Contenuto di RegexBasedTransformer.cs ###
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Transformers;

public abstract class RegexBasedTransformer : ITextTransformer
{
    protected List<(string pattern, string replacement)> RegexReplacements { get; set; } = new List<(string, string)>();

    public virtual string Transform(string input)
    {
        foreach (var (pattern, replacement) in RegexReplacements)
        {
            input = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        return input;
    }
}


### Contenuto di TransformerFactory.cs ###
using System;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Transformers
{
    public static class TransformerFactory
    {
        public static ITextTransformer Create(TransformerType type)
        {
            return type switch
            {
                TransformerType.Messenger => new ParserMessenger(),
                TransformerType.Instagram => new ParserInstagram(),
                TransformerType.Telegram => new ParserTelegram(),
                TransformerType.IoLeiCiclico => new ParserIoLeiCiclico(),
                TransformerType.Evernote => new ParserEvernote(),
                // I transformer per conversazioni esistenti vengono riutilizzati
                TransformerType.MarkdownToWiki => new MarkdownToWikiConverter(),
                _ => throw new NotImplementedException()
            };
        }
    }
}


### Contenuto di TransformerType.cs ###
namespace AggregaConversazioni.Transformers
{
    public enum TransformerType
    {
        Messenger,
        Instagram,
        Telegram,
        IoLeiCiclico,
        Evernote,
        Facebook,
        MarkdownToWiki // <-- Aggiunto
    }
}


### Contenuto di SpeakerIdentification.cs ###
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


