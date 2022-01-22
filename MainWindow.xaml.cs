using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using AggregaConversazioni;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<RigaDivisaPerPersone> Righe { get; set; } =
            new ObservableCollection<RigaDivisaPerPersone>();

        private void MessengerButton_Click(object sender, RoutedEventArgs e)
        {
            Parse(ParserMessenger.AnalizzaMessenger);
        }

        private void InstagramButton_Click(object sender, RoutedEventArgs e)
        {
            Parse(ParserInstagram.AnalizzaInstagram);
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Parse(ParserTelegram.AnalizzaTelegram);
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

        private void Parse(Func<string, (string text, IEnumerable<RigaDivisaPerPersone> k, List<string> speakers)> analizza)
        {
            Righe.Clear();
            var text = TextBox1.Text;

            var (outputText, k, speakers) = analizza(text);

            foreach (string speaker in speakers ?? new List<string>())
            {
                Speakers.Items.Add(speaker);
            }

            TextBox1.Text = outputText;
        }

        private void AnalizzaEvernote()
        {
            Righe.Clear();
            var text = TextBox1.Text;

            var outputText = ParserEvernote.AnalizzaEvernote(text);

            TextBox1.Text = outputText;
        }

        
    }
}
