using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWGen
{
    public class Passwort
    {
        string pw1;
        string pw2;
        int seed;
        string output;
        bool[] options = new bool[4];

        public Passwort(string pw1, string pw2, int seed, string output, bool gb, bool kb, bool sz, bool z)
        {
            this.pw1 = pw1;
            this.pw2 = pw2;
            this.seed = seed;
            this.output = output;
            options[0] = gb;
            options[1] = kb;
            options[2] = sz;
            options[3] = z;
        }

        public string Pw1 { get => pw1; set => pw1 = value; }
        public string Pw2 { get => pw2; set => pw2 = value; }
        public int Seed { get => seed; set => seed = value; }
        public string Output { get => output; set => output = value; }
        public bool[] Options { get => options; set => options = value; }
        public static Passwort Empty{
            get => new Passwort("", "", 0, "", true, true, true, true);
        }
    }
}
