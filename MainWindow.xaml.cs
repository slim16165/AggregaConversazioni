using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using AggregaConversazioni.Parser;
using Telerik.Windows.Controls;

namespace AggregaConversazioni;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        debugGrid2 = debugGrid;
    }

    public ObservableCollection<RigaDivisaPerPersone> Righe { get; set; } = new();
        
    public static ObservableCollection<RegexDebugData> RegexDebug { get; set; } = new();

    public static RadGridView debugGrid2;


    private void MessengerButton_Click(object sender, RoutedEventArgs e)
    {
        ParseAndDisplay(ParserFactory.Create(ParserType.Messenger));
    }

    private void InstagramButton_Click(object sender, RoutedEventArgs e)
    {
        ParseAndDisplay(ParserFactory.Create(ParserType.Instagram));
    }

    private void TelegramButton_Click(object sender, RoutedEventArgs e)
    {
        ParseAndDisplay(ParserFactory.Create(ParserType.Telegram));
    }

    private void IoLeiCiclico_Click(object sender, RoutedEventArgs e)
    {
        ParseAndDisplay(ParserFactory.Create(ParserType.IoLeiCiclico));
    }

    private void EvernoteButton_Click(object sender, RoutedEventArgs e)
    {
        AnalizzaEvernote();
    }

    private void FacebookButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ParseAndDisplay(ParserBase selectedParser)
    {
        Righe.Clear();
        var text = Input.Text;

        var (outputText, rigaDivisaPerPersone, speakers) = selectedParser.Parse(text);
        DisplaySpeakers(speakers);
        Input.Text = outputText;
        //Output.Text = selectedParser.DebugOutputTable;
    }

    private void DisplaySpeakers(List<string> speakers)
    {
        foreach (string speaker in speakers ?? new List<string>())
        {
            Speakers.Items.Add(speaker);
        }
    }

    private void AnalizzaEvernote()
    {
        Righe.Clear();
        var text = Input.Text;

        var outputText = ParserEvernote.AnalizzaEvernote(text);

        Input.Text = outputText;
    }
}