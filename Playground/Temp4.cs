using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliApp
{
    using System.Collections.Generic;

    class AppBase
    {

        public static List<string> DefaultPatterns = new() { "*", "Run *", "* Handler" };
        public static void AddCommandNamePattern(string pattern) { }
        public static void RemoveCommandNamePattern(string pattern) { }
    }
}
