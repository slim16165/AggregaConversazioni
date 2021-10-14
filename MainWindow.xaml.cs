using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Righe.Clear();
            var t = TextBox1.Text;
            var lines = Regex.Split(t, @"(\n|\r)+").Select(o => o.Trim()).Where(o => !string.IsNullOrWhiteSpace(o));

            var actors = new[] { "Persona1", "Persona2", "Persona3" };
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            //var output = new Dictionary<string, TextBox>();
            //output.Add(actors[0], TextBox2);
            //output.Add(actors[1], TextBox3);
            //output.Add(actors[2], TextBox4);
            //foreach (var pair in output)
            //{
            //    pair.Value.Text = "";
            //}

            var output = new Dictionary<string, Action<RigaDivisaPerPersone, string>>
            {
                { actors[0], (p, v) => p.Persona1 = v },
                { actors[1], (p, v) => p.Persona2 = v},
                { actors[2], (p, v) => p.Persona3 = v}
            };

            string currentBin = actors[0]; //Se non so chi sta parlando per primo scelgo a caso

            foreach (string curLine in lines)
            {
                //la riga corrente è il nome di una persona che parla
                if (actors.Contains(curLine))
                {
                    //if (output.ContainsKey(currentBin))
                    {
                        //output[currentBin].AppendText("\n\n");

                    }
                    currentBin = curLine;
                }
                else
                {
                    if (!dict.ContainsKey(currentBin) || dict[currentBin] == null)
                        dict[currentBin] = new List<string>();
                    else
                        dict[currentBin].Add(curLine);

                    //output[currentBin].AppendText(curLine);

                    var row = new RigaDivisaPerPersone();
                    var function = output[currentBin];

                    function(row, curLine);

                    ////if(currentBin)
                    //var k2 = new Func<RigaDivisaPerPersone, string, object>((p, v) => p.Persona1 = v);
                    //var k3 = new Action<RigaDivisaPerPersone, string>((p, v) => p.Persona1 = v);

                    Righe.Add(row);
                }
            }
        }
    }
}
