using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleSupport
{
    public static class BuilderParser
    {

        private enum Status
        {
            Ok,
            EndOfDefinition,
            Error
        }

        // T1, T2, etc are args if in curlies in path
        // Could also support complex types for options and args
        // Need to work out DI

        private static IEnumerable<(string? name, Type type)> GetHandlerParameters(LambdaExpression handler)
               => handler.Parameters.Select(x => (x.Name, x.Type));


        public static Command Map(string def, Command rootCommand, Delegate del)
        {
            var parms = GetHandlerParameters(del);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create(del);
            return leafCommand;

            static IEnumerable<(string? name, Type type)> GetHandlerParameters(Delegate del)
                       => del.Method.GetParameters().Select(x => (x.Name, x.ParameterType));
        }

        public static Command Map(string def,
                        Command rootCommand,
                        Expression<Action> handler,
                        IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create(() => Console.WriteLine(handler.Compile()));
            return leafCommand;
         }

        public static Command Map<TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create(() => Console.WriteLine(handler.Compile()));
            return leafCommand;
        }

        public static Command Map<T1, TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<T1, TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x) => Console.WriteLine(handler.Compile()(x)));
            return rootCommand;
        }


        public static Command Map<T1, T2, TRet>(string def,
                                         Command rootCommand,
                                         Expression<Func<T1, T2, TRet>> handler,
                                         IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x, T2 y) => Console.WriteLine(handler.Compile()(x, y)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, TRet>(string def,
                                         Command rootCommand,
                                         Expression<Func<T1, T2, T3,TRet>> handler,
                                         IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3) => Console.WriteLine(handler.Compile()(x1, x2, x3)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, T4, TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<T1, T2, T3, T4,TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4)));
            return rootCommand;
        }


        public static Command Map<T1, T2, T3, T4, T5,TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<T1, T2, T3, T4, T5,TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4, T5 x5) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4, x5)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, T4, T5,T6, TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<T1, T2, T3, T4, T5,T6, TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4, x5, x6)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, T4, T5, T6, T7, TRet>(string def,
                                 Command rootCommand,
                                 Expression<Func<T1, T2, T3, T4, T5, T6,T7, TRet>> handler,
                                 IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4, x5, x6, x7)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(string def,
                         Command rootCommand,
                         Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet>> handler,
                         IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4, x5, x6, x7, x8)));
            return rootCommand;
        }

        public static Command Map<T1, T2, T3, T4, T5, T6, T7, T8, T9 ,TRet>(string def,
                         Command rootCommand,
                         Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9,TRet>> handler,
                         IEnumerable<Type>? diTypes = default)
        {
            var parms = GetHandlerParameters(handler);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create((T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8, T9 x9) => Console.WriteLine(handler.Compile()(x1, x2, x3, x4, x5, x6, x7, x8, x9)));
            return rootCommand;
        }



        private static Command GetCommand(Command rootCommand,
                                                       string def,
                                                       IEnumerable<(string? name, Type type)> parms)
        {
            rootCommand ??= new RootCommand();
            var (_, leafCommand, status, message) = FillCommandAndReturnNextPos(rootCommand, CommandLineStringSplitter.Instance.Split(def), parms);
            return status == Status.Error
                        ? throw new InvalidOperationException(message)
                        : leafCommand;
        }

        private static (string[] rest, Command command, Status status, string? message)
                    FillCommandAndReturnNextPos(Command command,
                                                IEnumerable<string> parts,
                                                IEnumerable<(string? name, Type type)> parms)
        {
            var rest = parts.Select(x => x.Trim()).ToArray();
            var leafCommand = command;
            while (rest.Length > 0)
            {
                (rest, var symbol, var status, var message) = rest[0][0] switch
                {
                    '<' => NewArgumentAndReturnNextPos(rest, parms),
                    '-' => NewOptionAndReturnNextPos(rest, parms),
                    _ => NewCommandAndReturnNextPos(rest, parms)
                };
                if (status == Status.Error)
                {

                }
                if (symbol is not null)
                {
                    command.Add(symbol);
                }
                if (status == Status.EndOfDefinition) // if we support -- EODef won't always be rest is empty
                {
                    break;
                }
                if (symbol is Command cmd)
                {
                    leafCommand = cmd;
                }
            }

            return (rest, leafCommand, Status.Ok, null);

            static (string[], Symbol? symbol, Status status, string? message)
                            NewArgumentAndReturnNextPos(string[] slice,
                                                        IEnumerable<(string? name, Type type)> parms)
            {
                (var name, var type, var status, var message) = ArgumentNameAndType(slice, parms);
                if (status == Status.Error)
                {
                    return (slice[1..], null, status, message);
                }
                // If the message returns successfully, name and type are set
                var arg = new Argument(name!)
                {
                    ArgumentType = type!
                };

                return (slice[1..], arg, Status.Ok, null);
            }

            static (string? name, Type? type, Status status, string? message)
                            ArgumentNameAndType(string[] slice,
                                                IEnumerable<(string? name, Type type)> parms)
            {
                if (slice[0][^1] != '>')
                {
                    return (null, null, Status.Error, $"Bracket for argument '{slice[0]}' not closed or there is an extra space");
                }
                var name = slice[0][1..^1];
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (null, null, Status.Error, $"The name of an argument cannot be null");
                }
                var parm = parms.FirstOrDefault(x => x.name == name);
                return string.IsNullOrWhiteSpace(parm.name)
                    ? (null, null, Status.Error, $"Arguments must match a parameter in the handler by name")
                    : (name, parm.type, Status.Ok, null);
            }

            static (string[], Symbol? symbol, Status status, string? message)
                            NewOptionAndReturnNextPos(string[] slice,
                                                      IEnumerable<(string? name, Type type)> parms)
            {
                // TODO: Add syntax for in line aliases. Hopefully "|" will parse correctly
                // TODO: Consider supporting -- to pass arguments on within the CLI. Does SCL support this?
                var name = slice[0].Replace("-", "");
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (slice[1..], null, Status.Error, $"The name of an option cannot be null");
                }

                if (slice[1][0] == '<')
                {
                    // The option has an explicit argument
                    (var argName, var type, var status, var message) = ArgumentNameAndType(slice, parms);
                    if (status == Status.Error)
                    {
                        return (slice[1..], null, status, message);
                    }
                    var option = new Option(name, argumentType: type)
                    {
                        ArgumentHelpName = argName ?? name
                    };
                    return (slice[1..], option, Status.Ok, null);
                }
                else
                {
                    var parm = parms.FirstOrDefault(x => x.name == name);
                    if (string.IsNullOrWhiteSpace(parm.name)) //  since name is not empty, if parm is empty it was not found
                    {
                        return (slice[1..], null, Status.Error, $"Options must match a parameter in the handler by name");
                    }
                    var option = new Option(name);
                    return (slice[1..], option, Status.Ok, null);
                }
            }

            static (string[], Symbol? symbol, Status status, string? message)
                            NewCommandAndReturnNextPos(string[] slice,
                                                       IEnumerable<(string? name, Type type)> parms)
            {
                // TODO: Consider whether to support aliases. Since that is rare, leaning to a different approach like "cloning"
                var name = slice[0];
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (slice[1..], null, Status.Error, $"The name of an command cannot be null");
                }

                var command = new Command(name);
                return FillCommandAndReturnNextPos(command, slice[1..], parms);
            }
        }
    }
}
