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
