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

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> range)
            where TKey: notnull
        {
            foreach (var pair in range)
            {
                dict.Add(pair.Key, pair.Value);
            }
        }
    }


}
