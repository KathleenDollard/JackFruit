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
            return pos < 0 
                ? (input, "") 
                : (input[..pos], input[(pos + 1)..]);
        }
    }
}
