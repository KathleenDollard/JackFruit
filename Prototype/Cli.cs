using CliApp;
using DemoHandlers;
using System.CommandLine;

namespace Prototype

{
    internal partial class CliApp : AppBase
    {
        public static int Run(string[] args)
        {
            CreateWithRootCommand(Handlers.StarTrek); 
            var cli = new CliApp();
            cli.RootCommand.AddSubCommand(Handlers.NextGeneration);
            cli.RootCommand.NextGeneration.AddSubCommand(Handlers.Voyager);

            // Issue: With the following, is it confusing to require using System.CommandLiner
            return cli.RootCommand.Invoke(args);
        }
    }
}

