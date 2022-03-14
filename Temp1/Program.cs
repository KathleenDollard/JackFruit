using CliApp;
using DemoHandlers;
using CliDefinition;
using Generator.ConsoleSupport;
using System.CommandLine;

namespace CliDefinition
{
    public class Program
    {

        static async Task<int> Main(string[] args)
        {
            var app = ConsoleApplication.CreateWithRootCommand(Handlers.StarTrek);
            var starTrek = app.RootCommand;
            starTrek.AddSubCommand(Handlers.NextGeneration);
            starTrek.NextGeneration.AddSubCommand(Handlers.Voyager);
            starTrek.NextGeneration.AddSubCommand(Handlers.DeepSpaceNine);
            return await starTrek.Run(args); 
        }
    }
}

