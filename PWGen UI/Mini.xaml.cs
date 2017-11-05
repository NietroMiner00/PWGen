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
using System.Windows.Shapes;

namespace PWGen
{
    /// <summary>
    /// Interaktionslogik für Mini.xaml
    /// </summary>
    public partial class Mini : Window
    {
        MainWindow parent;
        public Mini(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 250;
            Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100;
            Load();
        }

        public void Load()
        {
            this.comboBox1.Items.Clear();
            foreach (string i in MainWindow.list.Keys)
            {
                this.comboBox1.Items.Add(i);
            }
            if (MainWindow.list.Keys.Count > 0) this.comboBox1.SelectedIndex = 0;
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            Passwort pw = Passwort.Empty;
            if (MainWindow.list.TryGetValue(comboBox1.SelectedItem.ToString(), out pw))
                Clipboard.SetText(pw.Output);
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            parent.mMini_Click(sender, e);
        }

        private void max_Click(object sender, RoutedEventArgs e)
        {
            parent.Show();
            Hide();
        }
    }
}
