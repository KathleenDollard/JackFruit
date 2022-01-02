using System.Linq.Expressions;
using System.CommandLine;
using System.Reflection.Metadata;

namespace ConsoleSupport
{
    public class ConsoleApplication
    {
        public RootCommand RootCommand { get; init; }
        public Dictionary<string, string> CommonAliases { get; init; }
        public ConsoleApplication(RootCommand? rootCommand = null)
        {
            RootCommand = rootCommand ?? new RootCommand();
            CommonAliases = new Dictionary<string, string>();
        }

        public ConsoleApplication AddCommonAlias(string alias, string optionName)
        {
            CommonAliases.Add(alias, optionName);
            return this;
        }

        public Command? MapInferred(string def, Delegate? del) { return null; }

        public void UseExceptionHandler(string? path = null) { }

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
    }
}
