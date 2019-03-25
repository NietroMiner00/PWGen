using Newtonsoft.Json;
using System;
using System.IO;

namespace PWGen
{
    class Options
    {
        private string passDir = Environment.GetEnvironmentVariable("userprofile") + "\\Documents\\PWGen\\";
        public string PassDir {
            get { return passDir; }
            set
            {
                passDir = value;
                saveOptions();
            }
        }
		private string passFile = "passwords.bin";
        public string PassFile
        {
            get { return passFile; }
            set
            {
                passFile = value;
                saveOptions();
            }
        }
        private string configFile = Environment.GetEnvironmentVariable("userprofile") + "\\Documents\\PWGen\\" + "config.json";
        public string ConfigFile
        {
            get { return configFile; }
            set
            {
                configFile = value;
                saveOptions();
            }
        }
        private bool showMini = true;
        public bool ShowMini
        {
            get { return showMini; }
            set
            {
                showMini = value;
                saveOptions();
            }
        }

        public void saveOptions()
        {
            string options = JsonConvert.SerializeObject(this);
            File.WriteAllText(configFile, options);
        }

        public void loadOptions()
        {
            if (File.Exists(configFile))
            {
				string options = File.ReadAllText(configFile);
                Options a = JsonConvert.DeserializeObject<Options>(options);
                passDir = a.passDir;
                passFile = a.passFile;
                showMini = a.showMini;
            }
        }


    }
}
