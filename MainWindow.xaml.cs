using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

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
            AnalizzaMessenger();
        }

        private void EvernoteButton_Click(object sender, RoutedEventArgs e)
        {
            AnalizzaEvernote();
        }

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InstagramButton_Click(object sender, RoutedEventArgs e)
        {
            AnalizzaInstagram();
        }

        private void AnalizzaMessenger()
        {
            Righe.Clear();
            var text = TextBox1.Text;

            List<string> speakers;
            var (outputText, k) = Parser.AnalizzaMessenger2(text);

            foreach (string speaker in speakers)
            {
                Speakers.Items.Add(speaker);
            }

            TextBox1.Text = outputText;
        }

        private static void AnalizzaInstagram()
        {
            Righe.Clear();
            var text = TextBox1.Text;
            var lines = Regex.Split(text, @"(\n|\r)+").Select(o => o.Trim());
            //Le regex sono le seguenti: ^(You sent|Stephanie replied to you|Original message:|Stephanie Frogs|Stephanie|)


            var speakers = Parser.IdentifySpeakers(lines);

            foreach (string speaker in speakers)
            {
                Speakers.Items.Add(speaker);
            }

            string shortSpeakerName;
            string longSpeakerName = "Stephanie Frogs";
            //shortSpeakerName = "Julia";
            longSpeakerName = "Julia Margini";
            longSpeakerName = "Francesco Arnaldi";
            longSpeakerName = "Sara Stefanile";
            shortSpeakerName = longSpeakerName.Split(' ').First();

            (string search, string replace) regexGiornoEOra = 
                (search: @"(Lunedì|Martedì|Mercoledì|Giovedì|Sabato|Domenica) \d{1,2}:\d{2}", replace: "");
          
            (string search, string replace) regexUsernameInterloc = 
                (search: "^Immagine del profilo di ([^ ]+)$", replace: "$1: ");

            text = Regex.Replace(text, regexGiornoEOra.search+"[\n\r]+", regexGiornoEOra.replace, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+{longSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, @"You sent[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{shortSpeakerName} replied to you[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"You replied to {shortSpeakerName}[\n\r]+", "Io: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{shortSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            text = Regex.Replace(text, $"{longSpeakerName}[\n\r]+", "Lei: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //elimino le ore 
            text = Regex.Replace(text, @"^\d{1,2}:\d{2} [ap]m[\n\r]", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Regex regexObj2 = new Regex(@"^(Io|Lei): (.*)\n\k<1>: ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            for (int i = 0; i < 10; i++)
            {
                text = regexObj2.Replace(text, @"$1: $2 ");
            }

            TextBox1.Text = text;

            
                }
            }
        }
    }
}
