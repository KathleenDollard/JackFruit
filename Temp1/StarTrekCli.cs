using CliApp;
using DemoHandlers;

namespace CliDefinition
{
    public partial class StarTrekCli : AppBase
    {
        internal StarTrekCli()
        {
            AddRootCommand(Handlers.StarTrek);
            StarTrekCli.AddSubCommand(Handlers.NextGeneration);
            //NextGeneration.AddSubCommand(Handlers.Voyager);
        }
    }
}
