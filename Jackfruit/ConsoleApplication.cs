using System;
using System.Collections.Generic;
using System.Reflection;
using ConsoleSupport;
using Microsoft.FSharp.Core;
using System.CommandLine;

namespace ConsoleSupport
{
    public class ConsoleApplication
    {
        internal RootCommand rootCommand;

        internal Dictionary<string, string> commonAliases;

        public RootCommand RootCommand => rootCommand;

        public Dictionary<string, string> CommonAliases => commonAliases;

        public ConsoleApplication(RootCommand rootCommand)
        {
            this.rootCommand = rootCommand;
            commonAliases = new Dictionary<string, string>();
        }

        public ConsoleApplication()
            : this(new RootCommand())
        {        }

        public ConsoleApplication AddCommonAlias(string alias, string optionName)
        {
            CommonAliases.Add(alias, optionName);
            return this;
        }

        public ConsoleApplication MapInferred(string def, Delegate? del)
        {
            // Currently used for code gen,  expect future implementation, 
            return this;
        }

        public ConsoleApplication UseExceptionHandler([OptionalArgument] FSharpOption<string> path)
        {
            // Expect future implementation
            return this;
        }

        public int Run(string[] args)
        {
            return 42;
        }

        public int Run(string args)
        {
            string text = ((!string.Equals(args, null)) ? args : "");
            return 42;
        }

        public int RunInLoop(string[] args, string message)
        {
            var ret = 0;
            if (args is not null)
            {
                ret = Run(args);
            }
            message ??= "Experiment with your CLI! Ctl-Z or Ctl-C to exit.";
            while (true)
            {
                Console.WriteLine(message);
                var input = Console.ReadLine();
                if (input is null)
                {
                    break;
                }
                ret = Run(input);
            }
            return ret;
        }

        public int RunInLoop(string[] args)
        {
            return RunInLoop(args, "Experiment with your CLI! Ctl-Z or Ctl-C to exit.");
        }
    }
}

