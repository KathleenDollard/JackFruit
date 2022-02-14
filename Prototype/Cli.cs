using CliApp;
using DemoHandlers;
using System.CommandLine;

namespace Prototype

{
    internal partial class CliApp : AppBase
    {
        public static int Run(string[] args)
        {
            var cli = CliApp.Create(Handlers.StarTrek);
            cli.RootCommand.AddSubCommand(Handlers.NextGeneration);
            cli.RootCommand.NextGeneration.AddSubCommand(Handlers.Voyager);

            // Issue: With the following, is it confusing to require using System.CommandLiner
            return cli.RootCommand.Invoke(args);
        }
    }
}

