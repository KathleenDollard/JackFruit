using System.CommandLine.Parsing;
using System.CommandLine.Invocation;

// START HERE: Handle arguments specified as parts on options and aliases on options.

namespace ConsoleSupport
{
    public static class BuilderInferredParser
    {

        private enum Status
        {
            Ok,
            EndOfDefinition,
            Error,
            InProcess
        }

        private enum PartClassification
        {
            NotFound,
            Arg,
            Option,
            Command
        }

        public static Command MapInferred(string def, Command rootCommand, Delegate del)
        {
            var parms = GetHandlerParameters(del);
            var leafCommand = GetCommand(rootCommand, def, parms);
            leafCommand.Handler = CommandHandler.Create(del);
            return leafCommand;

            static IEnumerable<(string? name, Type type)> GetHandlerParameters(Delegate del)
                       => del.Method.GetParameters().Select(x => (x.Name, x.ParameterType));
        }

        private static Command GetCommand(Command rootCommand,
                                                       string def,
                                                       IEnumerable<(string? name, Type type)> parms)
        {
            rootCommand ??= new RootCommand();

            IEnumerable<(string name, PartClassification partClass)> parts = CommandLineStringSplitter.Instance.Split(def)
                            .Select(x => (x, ClassifyPart(x)));

            var (_, leafCommand, status, message) = FillCommandAndReturnNextPos(rootCommand, parts, parms);
            return status == Status.Error
                        ? throw new InvalidOperationException(message)
                        : leafCommand;

            static PartClassification ClassifyPart(string part)
                => (part[0], part[1]) switch
                {
                    ('<', _) => PartClassification.Arg,
                    ('-', '-') => PartClassification.Option,
                    ('-', _) => throw new InvalidOperationException("Single dashes not allowed in definitions"),
                    _ => PartClassification.Command,

                };

        }

        private static (Command command, Status status, string? message)
                    FillCommandAndReturnNextPos(Command command,
                                                IEnumerable<(string name, PartClassification partClass)> parts,
                                                IEnumerable<(string? name, Type type)> parms)
        {
            var leafCommand = command;
            foreach (var parm in parms)
            {
                var part = parts.FirstOrDefault(x => x.name == parm.name);
                var partClass = part.partClass == PartClassification.NotFound
                                ? PartClassification.Option
                                : part.partClass;
                var (symbol, status, message) = partClass switch
                {
                    PartClassification.Arg => NewArgument(parm),
                    PartClassification.Option => NewOption(parm),
                    PartClassification.Command => NewCommand(parm),
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

            return (leafCommand, Status.Ok, null);

            static (Symbol? symbol, Status status, string? message)
                            NewArgument((string? name, Type type) parm)
            {
                var arg = new Argument(parm.name!)
                {
                    ArgumentType = parm.type
                };

                return (arg, Status.Ok, null);
            }

            static (Symbol? symbol, Status status, string? message)
                         NewOption((string? name, Type type) parm)
            {
                // TODO: Add syntax for in line aliases. Hopefully "|" will parse correctly
                // TODO: Consider supporting -- to pass arguments on within the CLI. Does SCL support this?
                if (string.IsNullOrWhiteSpace(parm.name))
                {
                    return ( null, Status.Error, $"The name of an option cannot be null");
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

            static (Symbol? symbol, Status status, string? message)
                             NewCommand((string? name, Type type) parm)
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
