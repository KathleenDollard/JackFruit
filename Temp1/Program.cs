using CliApp;
using DemoHandlers;
using CliDefinition;

namespace CliDefinition
{
    public class Program
    {

        static async Task<int> Main(string[] args)
        {
            return await CliDefinition.StarTrekApp.Run(args);
        }
    }
}

