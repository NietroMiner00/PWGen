using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWGen{

    public class Generator{

	    Random r;
	    char[] chars;
	    bool sz = true;
	    bool z = true;
	    bool gb = true;
	    bool kb = true;

        public bool setSZ(bool sz)
        {
            this.sz = sz;
            return this.sz;
        }
        public bool setZ(bool z)
        {
            this.z = z;
            return this.z;
        }
        public bool setGB(bool gb)
        {
            this.gb = gb;
            return this.gb;
        }
        public bool setKB(bool kb)
        {
            this.kb = kb;
            return this.kb;
        }
	
	    public string generate(string pw1, string pw2, int seed, int length){

            DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan javaSpan = DateTime.UtcNow - Jan1970;
            long currentTimeMillis = (long)javaSpan.TotalMilliseconds;
		
		    chars = new char[length];
		
		    for(int i = 0; i < pw1.ToCharArray().Length; i++){
			
			    seed += pw1.ToCharArray()[i];
			
		    }
		    for(int i = 0; i < pw2.ToCharArray().Length; i++){
			
			    seed += pw2.ToCharArray()[i];
			
		    }
		    if(pw1.Equals("")&&pw2.Equals("")) r = new Random((int)currentTimeMillis);
		    else r = new Random(seed);
		    for(int i = 0; i < length;i++){
			    chars[i] = (char)r.next(32);
			    for(int x = 0; x < Math.Abs(r.next(32)); x++){

                    chars[i] += (char)1;
                    if ((int)chars[i] > 126) chars[i] = (char)33;
                    if (!sz && (int)chars[i] >= 33 && (int)chars[i] <= 47) chars[i] = (char)48;
                    if (!z && (int)chars[i] >= 48 && (int)chars[i] <= 57) chars[i] = (char)58;
                    if (!sz && (int)chars[i] >= 58 && (int)chars[i] <= 64) chars[i] = (char)65;
                    if (!gb && (int)chars[i] >= 65 && (int)chars[i] <= 90) chars[i] = (char)91;
                    if (!sz && (int)chars[i] >= 91 && (int)chars[i] <= 96) chars[i] = (char)97;
                    if (!kb && (int)chars[i] >= 97 && (int)chars[i] <= 122) chars[i] = (char)123;
				    if(!sz&&(int)chars[i] >= 123 && (int)chars[i] <= 126){
                        if (z) chars[i] = (char)48;
                        else if (gb) chars[i] = (char)65;
                        else if (kb) chars[i] = (char)97;
				    }
				
				
			    }
		    }
		    String pw = "";
		    for(int i = 0; i < length;i++)
			    pw += chars[i];
		    return pw;
	    }
	
    }

}

