using PWGen_UI;
using System;
using System.Collections.Generic;
using System.IO;
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
        int passwort = 0;
        string passwordDir = Environment.GetEnvironmentVariable("userprofile") + "\\Documents\\PWGen\\";
        string passwordFile = "passwords.bin";
        bool temp = true;

        public MainWindow()
        {
            InitializeComponent();
            load(passwordDir + passwordFile);
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
            save();
            mypws_Click(null, null);
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

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            Passwort pw;
            if (passwords.SelectedItem != null)
            {
                list.TryGetValue(passwords.SelectedItem.ToString(), out pw);
                pw1.Text = pw.Pw1;
                pw2.Text = pw.Pw2;
                seed.Text = "" + pw.Seed;
                output.Text = pw.Output;
                length.Text = "" + pw.Output.Length;
                gb.IsChecked = pw.Options[0];
                kb.IsChecked = pw.Options[1];
                sz.IsChecked = pw.Options[2];
                z.IsChecked = pw.Options[3];
                name.Text = passwords.SelectedItem.ToString();
                error.Text = "";
            }
            create_Click(null, null);
        }

        private void save()
        {
            string pwstr = "";
            if (!temp)
            {
                bool ok = InputBox.Input("Geb dein Masterpasswort ein:", out pwstr);
                while (ok && (pwstr.GetHashCode() != this.passwort && this.passwort != 0))
                    ok = InputBox.Input("Geb dein Masterpasswort ein:", out pwstr);
                if (ok && (pwstr.GetHashCode() == this.passwort || this.passwort == 0))
                {
                    if (!System.IO.Directory.Exists(passwordDir)) System.IO.Directory.CreateDirectory(passwordDir);
                    FileStream fs = new FileStream(passwordDir + passwordFile, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write("PWGenFile");
                    for (int i = 0; i < passwords.Items.Count; i++)
                    {
                        Passwort pw;
                        list.TryGetValue(passwords.Items[i].ToString(), out pw);
                        bw.Write(AESEncryption.Encrypt(passwords.Items[i].ToString(), pwstr));
                        bw.Write(AESEncryption.Encrypt(pw.Pw1, pwstr));
                        bw.Write(AESEncryption.Encrypt(pw.Pw2, pwstr));
                        bw.Write(AESEncryption.Encrypt("" + pw.Seed, pwstr));
                        bw.Write(AESEncryption.Encrypt(pw.Output, pwstr));
                        bw.Write(AESEncryption.Encrypt("" + pw.Options[0], pwstr));
                        bw.Write(AESEncryption.Encrypt("" + pw.Options[1], pwstr));
                        bw.Write(AESEncryption.Encrypt("" + pw.Options[2], pwstr));
                        bw.Write(AESEncryption.Encrypt("" + pw.Options[3], pwstr));
                    }
                    bw.Write("!?!");
                    bw.Close();
                }
                else
                {
                    error.Text = "Falsches Passwort!";
                }
            }
            else
            {
                error.Text = "Temporäre Session aktiviert! Passwörter können nicht gespeichert werden!";
            }
        }

        private void load(string file)
        {
            if (System.IO.File.Exists(file))
            {
                FileStream fs1 = new FileStream(file, FileMode.Open);
                BinaryReader br = new BinaryReader(fs1);
                if (br.ReadString().Equals("PWGenFile"))
                {
                    string pwstr = "";
                    bool ok = false;
                    long startPosition = fs1.Position;
                    while (true)
                    {
                        ok = InputBox.Input("Geb dein Masterpasswort ein:", out pwstr);
                        if (ok)
                        {
                            this.passwort = pwstr.GetHashCode();
                            string name = br.ReadString();
                            try
                            {
                                while (!name.Equals("!?!"))
                                {
                                    Passwort pw = new Passwort(AESEncryption.Decrypt(br.ReadString(), pwstr),
                                        AESEncryption.Decrypt(br.ReadString(), pwstr),
                                        Int32.Parse(AESEncryption.Decrypt(br.ReadString(), pwstr)),
                                        AESEncryption.Decrypt(br.ReadString(), pwstr),
                                        Boolean.Parse(AESEncryption.Decrypt(br.ReadString(), pwstr)),
                                        Boolean.Parse(AESEncryption.Decrypt(br.ReadString(), pwstr)),
                                        Boolean.Parse(AESEncryption.Decrypt(br.ReadString(), pwstr)),
                                        Boolean.Parse(AESEncryption.Decrypt(br.ReadString(), pwstr)));
                                    list.Add(AESEncryption.Decrypt(name, pwstr), pw);
                                    passwords.Items.Add(AESEncryption.Decrypt(name, pwstr));

                                    temp = false;
                                    name = br.ReadString();
                                }
                                error.Text = "";
                                break;
                            }
                            catch (Exception e)
                            {
                                error.Text = "Falsches Passwort! Temporäre Session aktiviert! Passwörter können nicht gespeichert werden!";
                                fs1.Position = startPosition;
                            }
                        }
                        else
                        {
                            error.Text = "Temporäre Session aktiviert! Passwörter können nicht gespeichert werden!";
                            break;
                        }
                    }
                }
                fs1.Close();
            }
            else temp = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //save();
            e.Cancel = false;
        }
    }
}
