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
            private Dictionary<string, string> descriptions = new();
            private Dictionary<string, string> aliases = new();

            public Command Map<T1, T2, TRet>(string def,
                         Expression<Func<T1, T2, TRet>> handler,
                   string? description = null,
                   params string[] aliases)
            {
                var command = GetCommand(def, description, aliases);
                // T1, T2, etc are args if in curlies in path
                // Could also support complex types for options and args
                // Need to work out DI
                CommandWithOptionsArgs(command, handler);
                command.Handler = CommandHandler.Create((T1 x, T2 y)=>Console.WriteLine(handler.Compile()(x, y)));
                return command;

                Command CommandWithOptionsArgs(Command command, Expression<Func<T1, T2, TRet>> handler)
                {
                    AddArgOptionMaybe(command, def, handler.Parameters[0]);
                    AddArgOptionMaybe(command, def, handler.Parameters[1]);
                    return command;
                }
            }

            public Command Map<TRet>(string path,
                                     Expression<Func<TRet>> handler,
                                     string? description = null,
                                     params string[] aliases)
            {
                var command = GetCommand(path, description, aliases);
                command.Handler = CommandHandler.Create(() => Console.WriteLine(handler.Compile()()));
                return command;
            }


            public void Map(string path,
                            Action handler,
                            string? description = null,
                            params string[] aliases)
            {
                var command = GetCommand(path, description, aliases);
                command.Handler = CommandHandler.Create(handler);
            }



            //public void Map<TRet>(string path,
            //                      Func<TRet> handler,
            //                string? description = null,
            //                params string[] aliases)
            //{
            //    var command = GetCommand(path, description, aliases);
            //    //  var useHandler = WrapHandler(handler);
            //    command.Handler = CommandHandler.Create(handler);
            //}

            public void MapAsync<TArgs, TRet>(string path,
                                         Func<TArgs, TRet> handler,
                            string? description = null,
                            params string[] aliases)
            {
                var command = GetCommand(path, description, aliases);
                command.Handler = CommandHandler.Create(handler);
            }

            public void UseExceptionHandler(string? path = null)
            {
            }

            public void Run(string[] args)
            {
                rootCommand = rootCommand ?? throw new InvalidOperationException("No CLI commands defined");
                var output = rootCommand.Invoke(args);
            }

            public void Run(string args)
            {
                rootCommand = rootCommand ?? throw new InvalidOperationException("No CLI commands defined");
                var output = rootCommand.Invoke(args);
            }

            private Command GetCommand(string path,
                                       string? description,
                                       params string[] aliases)
            {
                rootCommand ??= new RootCommand();
                return GetCommand(rootCommand, PathSupport.Split(path), description, aliases);
            }

            private Command GetCommand(Command command,
                                       (string f, string r) t,
                                       string? description,
                                       params string[] aliases)
            {
                var (first, rest) = t;

                if (first.StartsWith("{"))
                {
                    return AddArgToCommand(command, first);
                }
                var candidate = string.IsNullOrWhiteSpace(first)
                                ? rootCommand
                                : command.Children
                                          .OfType<Command>()
                                          .Where(x => x.Name == first).FirstOrDefault();
                if (candidate is null)
                {
                    candidate = new Command(first);
                    command.AddCommand(candidate);
                }

                return string.IsNullOrWhiteSpace(rest)
                            ? CommandWithInfo(description, aliases)
                            : GetCommand(candidate, PathSupport.Split(rest), description, aliases);


                Command CommandWithInfo(string? desription, string[] aliases)
                {
                    candidate.Description = description;
                    foreach (var alias in aliases)
                    {
                        candidate.AddAlias(alias);
                    }
                    return candidate;
                }

                Command AddArgToCommand(Command command, string first)
                {
                    // Add the argument here, then update the type from the expression
                    command.AddArgument(new Argument(first.Replace("{", "").Replace("}", "")));
                    return command;
                }
            }

            public void AddDescriptions(Dictionary<string, string> descriptions)
                => this.descriptions = descriptions;

            public void AddAliases(Dictionary<string, string> aliases)
                => this.aliases = aliases;

            private Command AddArgOptionMaybe(Command command, string path, ParameterExpression param)
            {
                AddArgOptionMaybe(command, path, param.Name, param.Type);
                return command;
            }

            private Command AddArgOptionMaybe(Command command, string path, string? name, Type type)
            {
                if (command.Arguments[0].Name == name)
                {
                    command.Arguments[0].ArgumentType = type;
                    command.Arguments[0].Description = GetDescription(path);
                    return command;
                }
                // work out how we handle DI (might let SCL binder do it)
                var option = new Option($"--{name}" ?? "--<unknown>", "", type);
                option.Description = GetDescription(path);
                var alias = GetAlias(path);
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    option.AddAlias($"-{alias}");
                }
                command.AddOption(option);
                return command;
            }

            private string? GetDescription(string path)
                => descriptions.TryGetValue(path, out var description)
                        ? description
                        : null;

            private string? GetAlias(string path)
                => aliases.TryGetValue(path, out var alias)
                   ? alias
                   : null;

        }
    }
}
