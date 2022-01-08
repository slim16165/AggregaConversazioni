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

            var (outputText, k, speakers) = ParserMessenger.AnalizzaMessenger(text);

            foreach (string speaker in speakers)
            {
                Speakers.Items.Add(speaker);
            }

            TextBox1.Text = outputText;
        }

        private void AnalizzaInstagram()
        {
            Righe.Clear();
            var text = TextBox1.Text;

            var (outputText, k, speakers) = ParserInstagram.AnalizzaInstagram(text);

            foreach (string speaker in speakers)
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
