using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSupport
{
    public static class Extensions
    {
        public static int IndexOfStringEndsWith(this string[] slice, char c)
        {
            for (int i = 0; i < slice.Length; i++)
            {
                if (slice[i].EndsWith(c))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
