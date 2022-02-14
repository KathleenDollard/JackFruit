using CliApp;
using DemoHandlers;
using CliDefinition;

public class Program
{

    static void Main(string[] args)
    {
        //var app = DefineCli();
        return;

    }

    public void DefineCli()
    {
        {
            var app = AppBase.CreateWithRootCommand(Handlers.NextGeneration);
            app.OriginalSeries.AddSubCommand(Handlers.NextGeneration);
            app.OriginalSeries.NextGeneration.AddSubCommand(Handlers.Voyager);
            app.OriginalSeries.NextGeneration.AddSubCommand(Handlers.DeepSpaceNine);
        }
    }

}

