using PWGen_UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
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
        NotifyIcon notifyIcon = new NotifyIcon();
        System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
        System.Windows.Forms.MenuItem mExit = new System.Windows.Forms.MenuItem();
        System.Windows.Forms.MenuItem mShow = new System.Windows.Forms.MenuItem();
        System.Windows.Forms.MenuItem mMini = new System.Windows.Forms.MenuItem();
        static CloseReason closeReason = CloseReason.UserClosing;
        Mini miniForm;
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        public MainWindow()
        {
            Loaded += delegate
            {
                HwndSource source = (HwndSource)PresentationSource.FromDependencyObject(this);
                source.AddHook(WindowProc);
            };
            InitializeComponent();
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                closeReason = CloseReason.WindowsShutDown;
                Close();
                return;
            }
            mutex.ReleaseMutex();
            load(passwordDir + passwordFile);
            notifyIcon.Icon = new System.Drawing.Icon("ic_launcher.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "PWGen";
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseDoubleClick);
            this.notifyIcon.ContextMenu = this.cm;

            this.cm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mShow, this.mMini, this.mExit});

            this.mShow.Index = 0;
            this.mShow.Text = "Anzeigen";
            this.mShow.Click += new System.EventHandler(this.notifyIcon_MouseDoubleClick);

            // 
            // mMini
            // 
            this.mMini.Index = 1;
            this.mMini.Text = "Mini";
            this.mMini.Checked = true;
            this.mMini.Click += new System.EventHandler(this.mMini_Click);

            // 
            // mExit
            // 
            this.mExit.Index = 2;
            this.mExit.Text = "Beenden";
            this.mExit.Click += new System.EventHandler(this.mExit_Click);

            miniForm = new Mini(this);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOWME)
            {
                notifyIcon_MouseDoubleClick(null, null);
            }
            return IntPtr.Zero;
        }

        private static int LOWORD(int n)
        {
            return (n & 0xffff);
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
                if (output.Password.Equals("")) copy1.IsEnabled = false;
                error.Text = "";
                error.Text = "Es muss mindestens eine Checkbox ausgewählt sein!";
            }
            else
            {
                try
                {
                    if (Int32.Parse(length.Text) > 30) throw new FormatException();
                    output.Password = g.generate(pw1.Password, pw2.Password, Int32.Parse(seed.Text), Int32.Parse(length.Text));
                    error.Text = "";
                    copy1.IsEnabled = true;
                }
                catch (FormatException er)
                {
                    if (output.Password.Equals("")) copy1.IsEnabled = false;
                    error.Text = "";
                    error.Text = "Generierung nicht möglich aufgrund eines Formatierfehlers. Bitte sorgen sie dafür, dass der Startwert und die Länge(bis 30) Zahlen sind!";
                }
            }
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(output.Password);
            error.Text = "Passwort wurde in die Zwischenablage kopiert!";
        }

        private void empty_Click(object sender, RoutedEventArgs e)
        {
            pw1.Password = "";
            pw2.Password = "";
            seed.Text = "";
            error.Text = "";
            output.Password = "";
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
                list.Add(name.Text, new Passwort(pw1.Password, pw2.Password, Int32.Parse(seed.Text), output.Password, gb.IsChecked.Value, kb.IsChecked.Value, sz.IsChecked.Value, z.IsChecked.Value));
                passwords.Items.Add(name.Text);
            }
            else if (!name.Text.Equals(""))
            {
                list.Remove(name.Text);
                list.Add(name.Text, new Passwort(pw1.Password, pw2.Password, Int32.Parse(seed.Text), output.Password, gb.IsChecked.Value, kb.IsChecked.Value, sz.IsChecked.Value, z.IsChecked.Value));
            }
            save();
            mypws_Click(null, null);
            reload();
        }

        private void copy2_Click(object sender, RoutedEventArgs e)
        {
            Passwort pw = Passwort.Empty;
            if(list.TryGetValue(passwords.SelectedItem.ToString(), out pw))
                System.Windows.Clipboard.SetText(pw.Output);
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            list.Remove(passwords.SelectedItem.ToString());
            passwords.Items.Remove(passwords.SelectedItem.ToString());
            save();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            Passwort pw;
            if (passwords.SelectedItem != null)
            {
                list.TryGetValue(passwords.SelectedItem.ToString(), out pw);
                pw1.Password = pw.Pw1;
                pw2.Password = pw.Pw2;
                seed.Text = "" + pw.Seed;
                output.Password = pw.Output;
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        Passwort pw = list.ElementAt(i).Value;
                        bw.Write(AESEncryption.Encrypt(list.ElementAt(i).Key, pwstr));
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
            miniForm.Load();
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

        public void reload()
        {
            passwords.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                passwords.Items.Add(list.ElementAt(i).Key);
            }
        }

        bool firstClose = true;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (closeReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
                if (firstClose)
                {
                    firstClose = false;
                }
                if (mMini.Checked) miniForm.Show();
            }
        }

        public void notifyIcon_MouseDoubleClick(object sender, EventArgs e)
        {
            Show();
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            miniForm.Hide();
        }

        private void mExit_Click(object sender, System.EventArgs e)
        {
            closeReason = CloseReason.WindowsShutDown;
            Close();
            miniForm.Close();
            notifyIcon.Visible = false;
        }

        public void mMini_Click(object sender, EventArgs e)
        {
            mMini.Checked = !mMini.Checked;
            if (!mMini.Checked) miniForm.Hide();
            else miniForm.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!vis.IsChecked.Value)
            {
                pw1_vis.Visibility = Visibility.Hidden;
                pw2_vis.Visibility = Visibility.Hidden;
                output_vis.Visibility = Visibility.Hidden;
            }
            else
            {
                pw1_vis.Visibility = Visibility.Visible;
                pw2_vis.Visibility = Visibility.Visible;
                output_vis.Visibility = Visibility.Visible;
                pw1_vis.Text = pw1.Password;
                pw2_vis.Text = pw2.Password;
                output_vis.Text = output.Password;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(sender.Equals(pw1_vis)&&vis.IsChecked.Value) pw1.Password = pw1_vis.Text;
            if(sender.Equals(pw2_vis) && vis.IsChecked.Value) pw2.Password = pw2_vis.Text;
            if(sender.Equals(output_vis) && vis.IsChecked.Value) output.Password = output_vis.Text;
            if (sender.Equals(pw1) && !vis.IsChecked.Value) pw1_vis.Text = pw1.Password;
            if (sender.Equals(pw2) && !vis.IsChecked.Value) pw2_vis.Text = pw2.Password;
            if (sender.Equals(output) && !vis.IsChecked.Value) output_vis.Text = output.Password;
        }

        internal class NativeMethods
        {
            public const int HWND_BROADCAST = 0xffff;
            public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
            [DllImport("user32")]
            public static extern int RegisterWindowMessage(string message);
        }
    }
}
