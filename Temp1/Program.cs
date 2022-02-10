using CliApp;
using DemoHandlers;
using CliDefinition;

public class Program
{

    static void Main(string[] args)
    {
        var app = DefineCli();
        return;

    }

    static AppBase DefineCli()
    {
        AppBase.CreateWithRootCommand(Handlers.StarTrek);
        return app;
    }

}

