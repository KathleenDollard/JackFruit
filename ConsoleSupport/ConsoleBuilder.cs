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
        public App Build()
        {
            return new App();
        }

        public class App
        {
            private Command? rootCommand;
            private readonly Dictionary<string, string> descriptions = new();
            private readonly Dictionary<string, string> aliases = new();

            public Command RootCommand
                    => rootCommand ??= new RootCommand();

            public Command Map<T1, T2, TRet>(string def,
                         Expression<Func<T1, T2, TRet>> handler)
                => BuilderParser.Map(def, RootCommand, handler);

            public Command Map<T1, TRet>(string def,
                       Expression<Func<T1, TRet>> handler)
                => BuilderParser.Map(def, RootCommand, handler);

            public Command Map<TRet>(string def,
                                     Expression<Func<TRet>> handler)
                => BuilderParser.Map(def, RootCommand, handler);


            public Command Map(string def,
                            Expression<Action> handler)
                => BuilderParser.Map(def, RootCommand, handler);


            public void UseExceptionHandler(string? path = null)
            {
            }

            public int Run(string[] args)
            {
                rootCommand = rootCommand ?? throw new InvalidOperationException("No CLI commands defined");
                return rootCommand.Invoke(args);
            }

            public void Run(string? args)
            {
                rootCommand = rootCommand ?? throw new InvalidOperationException("No CLI commands defined");
                args ??= "";
                rootCommand.Invoke(args);
            }

            public static void Throw<T>(T exception)
                where T : Exception
                => throw exception;

            public void AddDescriptions(Dictionary<string, string> descriptions)
            {
                foreach (var desc in descriptions)
                {
                    this.descriptions.Add(desc.Key, desc.Value);
                }
            }
        }
    }
}
