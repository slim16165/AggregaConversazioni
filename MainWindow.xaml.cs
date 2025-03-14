using System.Windows;

namespace AggregaConversazioni;
// Classe per rappresentare il tipo di conversione

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}