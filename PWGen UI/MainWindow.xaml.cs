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
        public static SortedList<string, Passwort> list = new SortedList<string, Passwort>();

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

        private void empty_Click(object sender, RoutedEventArgs e)
        {
            pw1.Text = "";
            pw2.Text = "";
            seed.Text = "";
            error.Text = "";
            output.Text = "";
            length.Text = "10";
            name.Text = "";
            gb.IsChecked = true;
            kb.IsChecked = true;
            sz.IsChecked = true;
            z.IsChecked = true;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (!name.Text.Equals("") && !passwords.Items.Contains(name.Text))
            {
                list.Add(name.Text, new Passwort(pw1.Text, pw2.Text, Int32.Parse(seed.Text), output.Text, gb.IsChecked.Value, kb.IsChecked.Value, sz.IsChecked.Value, z.IsChecked.Value));
                passwords.Items.Add(name.Text);
            }
            else if (!name.Text.Equals(""))
            {
                list.Remove(name.Text);
                list.Add(name.Text, new Passwort(pw1.Text, pw2.Text, Int32.Parse(seed.Text), output.Text, gb.IsChecked.Value, kb.IsChecked.Value, sz.IsChecked.Value, z.IsChecked.Value));
            }
        }

        private void copy2_Click(object sender, RoutedEventArgs e)
        {
            Passwort pw = Passwort.Empty;
            if(list.TryGetValue(passwords.SelectedItem.ToString(), out pw))
                Clipboard.SetText(pw.Output);
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            list.Remove(passwords.SelectedItem.ToString());
            passwords.Items.Remove(passwords.SelectedItem.ToString());
        }
    }
}
