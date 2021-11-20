using System.Linq.Expressions;
using System.CommandLine;

namespace ConsoleSupport
{
    internal static class ConsoleApplicationExtensions
    {
        internal static Command AddToRootCommand(this Command command, Command rootCommand)
        {
            rootCommand.Add(command);
            return command;
        }

        internal static IEnumerable<Command> GetCommands(this Command command)
        {
            return command.Children.OfType<Command>();
        }
    }
    public class ConsoleApplication
    {
        private readonly Dictionary<string, string> descriptions = new();
        private readonly BuilderInferredParser builderParser;

        public ConsoleApplication(Command? rootCommand = null)
        {
            builderParser = new BuilderInferredParser();
            RootCommand = rootCommand ?? new RootCommand();
        }

        public ConsoleApplication AddCommonAlias(string alias, string optionName)
        {
            builderParser.AddCommonAlias(optionName, alias);
            return this;
        }

        public static ConsoleBuilder CreateBuilder()
            => new();

        public Command RootCommand { get; }

        public Command MapInferred(string def, Delegate? del)
            => CommandFromInfo(RootCommand, builderParser.MapInferred(def, del));

        public Command Map(string def,
                       Delegate del)
           => BuilderParser.Map(def, RootCommand, del);

        public Command Map(string def,
                        Expression<Action> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<TRet>(string def,
                                 Expression<Func<TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, TRet>(string def,
                   Expression<Func<T1, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, TRet>(string def,
                     Expression<Func<T1, T2, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, TRet>(string def,
             Expression<Func<T1, T2, T3, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);


        public Command Map<T1, T2, T3, T4, T5, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);


        public void UseExceptionHandler(string? path = null)
        {
        }

        public int Run(string[] args)
           => (RootCommand ?? throw new InvalidOperationException("No CLI commands defined"))
              .Invoke(args);

        public int Run(string? args)
           => (RootCommand ?? throw new InvalidOperationException("No CLI commands defined"))
              .Invoke(args ?? "");

        public int RunInLoop(string[]? args = null, string? message = null)
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

        public void Throw<T>(T exception)
            where T : Exception
            => throw exception;

        public void AddDescriptions(Dictionary<string, string> descriptions)
            => this.descriptions.AddRange(descriptions);

        private static Command CommandFromInfo(Command rootCommand, CommandInfo commandInfo)
        {
            if (string.IsNullOrWhiteSpace(commandInfo.Name))
            {
                // Assume this is the root
                return rootCommand;
            }
            var command = new Command(commandInfo.Name);
            var parent = FindParent(rootCommand, commandInfo);
            parent.Add(command);
            foreach (var option in commandInfo.Options)
            {
                command.Add(OptionFromInfo(option));
            }
            foreach (var arg in commandInfo.Arguments)
            {
                command.Add(ArgumentFromInfo(arg));
            }
            foreach (var alias in commandInfo.Aliases)
            {
                command.AddAlias(alias);
            }
            return command;

            static Command FindParent(Command rootCommand, CommandInfo command)
            {
                var parentCommand = rootCommand;
                foreach (var parent in command.Parents)
                {
                    if (string.IsNullOrWhiteSpace( parent.Name))
                    {
                        // assume we found the root and all is well
                        continue;
                    }
                    var foundCommand = parentCommand.GetCommands()
                                                 .FirstOrDefault(x => x.Name.Equals(parent.Name, StringComparison.OrdinalIgnoreCase));
                    if (foundCommand is null)
                    {
                        foundCommand = new Command(parent.Name);
                        parentCommand.Add(foundCommand);
                    }
                    parentCommand = foundCommand;
                }
                return parentCommand;
            }

            static Option OptionFromInfo(OptionInfo info)
            {
                var option = new Option(info.Name, argumentType: info.Type);
                foreach (var alias in info.Aliases)
                {
                    option.AddAlias(alias);
                }
                option.ArgumentHelpName = string.IsNullOrWhiteSpace(info.ArgHelpName)
                                            ? info.Name
                                            : info.ArgHelpName;
                return option;
            }

            static Argument ArgumentFromInfo(ArgInfo info)
            {
                var arg = new Argument(info.Name);
                //arg.ArgumentType = info.Type;
                return arg;
            }
        }
    }
}
