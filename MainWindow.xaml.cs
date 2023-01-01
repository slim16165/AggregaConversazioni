using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace AggregaConversazioni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<RigaDivisaPerPersone> Righe { get; set; } =
            new ObservableCollection<RigaDivisaPerPersone>();

        private void MessengerButton_Click(object sender, RoutedEventArgs e)
        {
            Parser p = new ParserMessenger();
            Parse(p);
        }

        private void InstagramButton_Click(object sender, RoutedEventArgs e)
        {
            Parser p = new ParserInstagram();
            Parse(p);
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Parser p = new ParserTelegram();
            Parse(p);
        }

        private void IoLeiCiclico_Click(object sender, RoutedEventArgs e)
        {
            Parse(Parser.ParseIo_LeiCiclico2);
        }

        private void EvernoteButton_Click(object sender, RoutedEventArgs e)
        {
            AnalizzaEvernote();
        }

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Parse(Parser p)
        {
            Righe.Clear();
            var text = Input.Text;

            var (outputText, k, speakers) = p.Parse(text);
            
            foreach (string speaker in speakers ?? new List<string>())
            {
                Speakers.Items.Add(speaker);
            }

            Input.Text = outputText;
            Output.Text = p.DebugOutputTable;
        }

        private void Parse(Func<string, (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers)> analizza)
        {
            Righe.Clear();
            var text = Input.Text;

            var (outputText, k, speakers) = analizza(text);

            foreach (string speaker in speakers ?? new List<string>())
            {
                Speakers.Items.Add(speaker);
            }

            Input.Text = outputText;
        }

        private void AnalizzaEvernote()
        {
            Righe.Clear();
            var text = Input.Text;

            var outputText = ParserEvernote.AnalizzaEvernote(text);

            Input.Text = outputText;
        }

        
    }
}
