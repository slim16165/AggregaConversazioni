using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AggregaConversazioni.Models;
using AggregaConversazioni.Transformers;
using PropertyChanged;

// Assicurati di avere il package Fody.PropertyChanged

namespace AggregaConversazioni;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    public string InputText { get; set; } = "";
    public string OutputText { get; set; } = "";
    public int InputTextLength => string.IsNullOrEmpty(InputText) ? 0 : InputText.Length;
    public int OutputTextLength => string.IsNullOrEmpty(OutputText) ? 0 : OutputText.Length;
    public string ConversionStats { get; set; } = "Pronto";
    public double ConversionProgress { get; set; }
    public bool ShowDebug { get; set; }

    public ObservableCollection<ConversionTypeItem> ConversionTypes { get; set; }
    public ConversionTypeItem SelectedConversionType { get; set; }
    public ObservableCollection<RegexDebugData> AppliedRules { get; set; }
    public ObservableCollection<string> Speakers { get; set; }

    public ICommand ConvertCommand { get; set; }
    public ICommand LoadSampleCommand { get; set; }

    public MainViewModel()
    {
        // Inizializza le collezioni e i comandi
        ConversionTypes = new ObservableCollection<ConversionTypeItem>
        {
            new() { DisplayName = "Facebook", Type = TransformerType.Facebook },
            new() { DisplayName = "Messenger", Type = TransformerType.Messenger },
            new() { DisplayName = "Instagram", Type = TransformerType.Instagram },
            new() { DisplayName = "Telegram", Type = TransformerType.Telegram },
            new() { DisplayName = "Evernote", Type = TransformerType.Evernote },
            new() { DisplayName = "Io/Lei ciclico", Type = TransformerType.IoLeiCiclico },
            new() { DisplayName = "Markdown to Wiki", Type = TransformerType.MarkdownToWiki },
            new() { DisplayName = "Markdown to Wiki Pandoc", Type = TransformerType.MarkdownToWikiPandoc }
        };
        SelectedConversionType = ConversionTypes[0];

        AppliedRules = new ObservableCollection<RegexDebugData>();
        Speakers = new ObservableCollection<string>();

        ConvertCommand = new RelayCommand(o => ConvertText(), o => !string.IsNullOrWhiteSpace(InputText));
        LoadSampleCommand = new RelayCommand(o => LoadSampleText());
    }

    private void ConvertText()
    {
        try
        {
            ConversionProgress = 0;
            ConversionStats = "Conversione in corso...";

            var transformer = TransformerFactory.Create(SelectedConversionType.Type);
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
        InputText = SelectedConversionType.Type switch
        {
            TransformerType.Messenger => "[10:00 AM] John: Ciao, come stai?\r\n[10:01 AM] Jane: Bene, grazie! E tu?",
            TransformerType.Instagram => "Foto di una spiaggia al tramonto. #tramonto #spiaggia @utente",
            TransformerType.Telegram => "[12:30] Alice: Hai visto il nuovo film?\r\n[12:31] Bob: Sì, è fantastico!",
            TransformerType.IoLeiCiclico => "Io: Ciao\r\nLei: Ciao, come va?\r\nIo: Bene, e tu?",
            TransformerType.Evernote => "Titolo della nota\r\n- Punto 1\r\n- Punto 2\r\nTesto della nota.",
            TransformerType.Facebook => "Ho appena visto un film fantastico! Guarda il trailer qui: [link]",
            TransformerType.MarkdownToWiki =>
                "# Titolo Principale\r\nTesto **in grassetto** e *in corsivo*.\r\n- Elemento 1\r\n- Elemento 2\r\n[Link](http://example.com)",
            TransformerType.MarkdownToWikiPandoc =>
                "# Titolo Principale\r\nTesto **in grassetto** e *in corsivo*.\r\n- Elemento 1\r\n- Elemento 2\r\n[Link](http://example.com)",
            _ => $"Esempio di testo per {SelectedConversionType.DisplayName}"
        };
    }
}