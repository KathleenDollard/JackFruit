using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleSupport
{
    public class ConsoleBuilder
    {

        // TODO: Support services here? Not sure who should own the host. 

        public ConsoleApplication Build()
        {
            return new ConsoleApplication ();
        }


    }
}
