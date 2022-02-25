using CliApp;
using DemoHandlers;
using System.CommandLine;

namespace CliDefinition
{
    internal partial class StarTrekApp : AppBase
    {
        public static async Task<int> Run(string[] args)
        {
            CreateWithRootCommand(Handlers.StarTrek);
            var cli = StarTrekApp.Create();
            cli.StarTrek.AddSubCommand(Handlers.NextGeneration);
            cli.StarTrek.NextGeneration.AddSubCommand(Handlers.Voyager);

            // Issue: With the following, is it confusing to require using System.CommandLiner
            return await cli.RootCommand.InvokeAsync(args);
        }
    }

}
