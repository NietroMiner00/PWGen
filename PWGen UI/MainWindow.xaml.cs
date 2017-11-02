using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PWGen
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Generator g = new Generator();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mypws_Click(object sender, RoutedEventArgs e)
        {
            PasswoerterPanel.Visibility = Visibility.Visible;
            ErstellenPanel.Visibility = Visibility.Hidden;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            ErstellenPanel.Visibility = Visibility.Visible;
            PasswoerterPanel.Visibility = Visibility.Hidden;
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            g.setGB(gb.IsChecked.Value);
            g.setKB(kb.IsChecked.Value);
            g.setSZ(sz.IsChecked.Value);
            g.setZ(z.IsChecked.Value);
            if (!g.setGB(gb.IsChecked.Value) &&
                !g.setKB(kb.IsChecked.Value) &&
                !g.setSZ(sz.IsChecked.Value) &&
                !g.setZ(z.IsChecked.Value))
            {
                if (output.Text.Equals("")) copy1.IsEnabled = false;
                error.Text = "";
                error.Text = "Es muss mindestens eine Checkbox ausgewählt sein!";
            }
            else
            {
                try
                {
                    if (Int32.Parse(length.Text) > 30) throw new FormatException();
                    output.Text = g.generate(pw1.Text, pw2.Text, Int32.Parse(seed.Text), Int32.Parse(length.Text));
                    error.Text = "";
                    copy1.IsEnabled = true;
                }
                catch (FormatException er)
                {
                    if (output.Text.Equals("")) copy1.IsEnabled = false;
                    error.Text = "";
                    error.Text = "Generierung nicht möglich aufgrund eines Formatierfehlers. Bitte sorgen sie dafür, dass der Startwert und die Länge(bis 30) Zahlen sind!";
                }
            }
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(output.Text);
            error.Text = "Passwort wurde in die Zwischenablage kopiert!";
        }
    }
}
