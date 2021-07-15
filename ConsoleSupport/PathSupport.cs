using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSupport
{
    public static class PathSupport
    {
        public static (string first, string rest) Split(string input)
        {
            var pos = input.IndexOf('/');
            if (pos < 0)
            {
                return (input, "");
            }
            return (input[..pos], input[(pos + 1)..]);
        }
    }
}
