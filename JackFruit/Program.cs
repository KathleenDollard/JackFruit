using ConsoleSupport;

var builder = ConsoleApplication.CreateBuilder(args);
// Add Services here
var app = builder.Build(); 
app.UseExceptionHandler();

FirstPlayground.Cli(app);

// You can just run, but this lets you play around. Definitely try help.
while (true)
{
    var input = Console.ReadLine();
    app.Run(input);
}