using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWGen
{
    class Random
    {
        AtomicLong seed;

        public Random(long seed)
        {
            this.seed = new AtomicLong((seed ^ 0x5DEECE66DL) & (1L << 48) - 1);
        }

        public int next(int bits) {
            long oldseed, nextseed;
            AtomicLong seed = this.seed;
            do {
                oldseed = seed.get();
                nextseed = (oldseed * 0x5DEECE66DL + 0xBL) & (1L << 48) - 1;
            } while (!seed.compareAndSet(oldseed, nextseed));
            return (int)(nextseed >> (48 - bits));
        }
    }
}
