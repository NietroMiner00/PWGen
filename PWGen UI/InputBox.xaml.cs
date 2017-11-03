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

namespace PWGen_UI
{
    /// <summary>
    /// Interaktionslogik für InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        bool okB = false;

        public InputBox()
        {
            InitializeComponent();
        }

        public static bool Input(string text, out string output)
        {
            InputBox i = new InputBox();
            i.text.Content = text;
            i.ShowDialog();
            if (i.okB)
            {
                output = i.input.Text;
                return true;
            }
            else output = "";
            return false;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            okB = true;
            Close();
        }
    }
}
