using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWGen
{
    class AtomicLong
    {
        long value;

        public AtomicLong(long value)
        {
            this.value = value;
        }

        public long get()
        {
            return value;
        }

        public Boolean compareAndSet(long expect, long update)
        {
            if (value == expect)
            {
                value = update;
                return true;
            }
            return false;
        }
    }
}
