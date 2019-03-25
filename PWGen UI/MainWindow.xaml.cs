using PWGen_UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

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
        bool temp = true;
        NotifyIcon notifyIcon = new NotifyIcon();
        ContextMenu cm = new ContextMenu();
        MenuItem mExit = new MenuItem();
        MenuItem mShow = new MenuItem();
        MenuItem mMini = new MenuItem();
        static CloseReason closeReason = CloseReason.UserClosing;
        Mini miniForm;
        Thread tester;
        Options opt;
        Passwort recent;

        public MainWindow()
        {
            Process[] pname = Process.GetProcessesByName("pwgen");
            if (pname.Length != 1)
            {
                Close();
                Console.WriteLine(pname.Length);
                return;
            }

            InitializeComponent();

            opt = new Options();
            opt.loadOptions();

            recent = new Passwort("", "", 0, "", true, true, true, true);

            load(opt.PassDir + opt.PassFile);

            notifyIcon.Icon = new System.Drawing.Icon(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "ic_launcher.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "PWGen";
            notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);
            this.notifyIcon.ContextMenu = this.cm;

            this.cm.MenuItems.AddRange(new MenuItem[] {
            this.mShow, this.mMini, this.mExit});

            this.mShow.Index = 0;
            this.mShow.Text = "Anzeigen";
            this.mShow.Click += new EventHandler(this.notifyIcon_MouseDoubleClick);

            // 
            // mMini
            // 
            this.mMini.Index = 1;
            this.mMini.Text = "Mini";
            this.mMini.Checked = opt.ShowMini;
            this.showMiniOption.IsChecked = opt.ShowMini;
            this.mMini.Click += new System.EventHandler(this.mMini_Click);

            // 
            // mExit
            // 
            this.mExit.Index = 2;
            this.mExit.Text = "Beenden";
            this.mExit.Click += new System.EventHandler(this.mExit_Click);

            miniForm = new Mini(this);

            pathInput.Text = opt.PassDir;

            tester = new Thread(TestForOtherProgramm);
            tester.Start();
        }

        private void TestForOtherProgramm()
        {
            while (true)
            {
                Process[] pname = Process.GetProcessesByName("pwgen");
                if (pname.Length != 1)
                {
                    this.Dispatcher.BeginInvoke((Action)(() => notifyIcon_MouseDoubleClick(null, null)));
                }
            }
        }

        private void mypws_Click(object sender, RoutedEventArgs e)
        {
            PasswoerterPanel.Visibility = Visibility.Visible;
            ErstellenPanel.Visibility = Visibility.Hidden;
            optionsPanel.Visibility = Visibility.Hidden;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            ErstellenPanel.Visibility = Visibility.Visible;
            PasswoerterPanel.Visibility = Visibility.Hidden;
            optionsPanel.Visibility = Visibility.Hidden;
        }

        private void options_Click(object sender, RoutedEventArgs e)
        {
            PasswoerterPanel.Visibility = Visibility.Hidden;
            ErstellenPanel.Visibility = Visibility.Hidden;
            optionsPanel.Visibility = Visibility.Visible;
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
                    if (!System.IO.Directory.Exists(opt.PassDir)) System.IO.Directory.CreateDirectory(opt.PassDir);
                    FileStream fs = new FileStream(opt.PassDir + opt.PassFile, FileMode.Create);
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
                                //Clear in case of reload
                                if(list.Count > 0||passwords.Items.Count > 0)
                                {
                                    list.Clear();
                                    passwords.Items.Clear();
                                }
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
                WindowState = WindowState.Minimized;
                this.Hide();
                e.Cancel = true;
                if (firstClose)
                {
                    firstClose = false;
                }
                if (opt.ShowMini) miniForm.Show();
            }
            else tester.Abort();
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
            opt.ShowMini = !opt.ShowMini;
            mMini.Checked = opt.ShowMini;
            showMiniOption.IsChecked = opt.ShowMini;
            if (!opt.ShowMini) miniForm.Hide();
            else miniForm.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!vis.IsChecked.Value)
            {
                pw1_vis.Visibility = Visibility.Hidden;
                pw2_vis.Visibility = Visibility.Hidden;
                output_vis.Visibility = Visibility.Hidden;
                pw1.Password = recent.Pw1;
                pw2.Password = recent.Pw2;
                output.Password = recent.Output;
            }
            else
            {
                pw1_vis.Visibility = Visibility.Visible;
                pw2_vis.Visibility = Visibility.Visible;
                output_vis.Visibility = Visibility.Visible;
                pw1_vis.Text = recent.Pw1;
                pw2_vis.Text = recent.Pw2;
                output_vis.Text = recent.Output;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(sender.Equals(pw1_vis)) recent.Pw1 = pw1_vis.Text;
            if(sender.Equals(pw2_vis)) recent.Pw2 = pw2_vis.Text;
            if(sender.Equals(output_vis)) recent.Output = output_vis.Text;
            if (sender.Equals(pw1))
            {
                recent.Pw1 = pw1.Password;
                pw1_vis.Text = recent.Pw1;
            }
            if (sender.Equals(pw2))
            {
                recent.Pw2 = pw2.Password;
                pw2_vis.Text = recent.Pw2;
            }
            if (sender.Equals(output))
            {
                recent.Output = output.Password;
                output_vis.Text = recent.Output;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                Show();
            }
            Console.WriteLine(WindowState.ToString());
        }

        private void searchPath_Click(object sender, RoutedEventArgs e)
        {
            //Change Password Directory
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Add "\\"
                if (!openFileDialog.SelectedPath.EndsWith("\\")) openFileDialog.SelectedPath += "\\";

                //Migrate old file
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Willst du deine Passwörter zum neuen Ort migrieren?", "Migrieren", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    if(File.Exists(openFileDialog.SelectedPath + opt.PassFile))
                    {
                        //Overwrite Dialog
                        DialogResult overwriteResult = System.Windows.Forms.MessageBox.Show("Willst du die vorhandene Datei überschreiben?", "Überschreiben", MessageBoxButtons.YesNo);
                        if (overwriteResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            File.Copy(opt.PassDir + opt.PassFile, openFileDialog.SelectedPath + opt.PassFile, true);
                        }
                    }
                    else
                        File.Copy(opt.PassDir + opt.PassFile, openFileDialog.SelectedPath + opt.PassFile);
                }
                else if (dialogResult == System.Windows.Forms.DialogResult.Abort)
                {
                    return;
                }

                //change Path
                pathInput.Text = openFileDialog.SelectedPath;
                opt.PassDir = openFileDialog.SelectedPath;
                load(opt.PassDir + opt.PassFile);
            }
        }

        private void showMiniOption_Checked(object sender, RoutedEventArgs e)
        {
            if (opt != null)
            {
                opt.ShowMini = showMiniOption.IsChecked.Value;
                mMini.Checked = opt.ShowMini;
            }
        }
    }
}
