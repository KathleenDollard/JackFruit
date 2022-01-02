using ConsoleSupport;

var app = new ConsoleApplication();

//FirstPlayground.DefineCli(app);
DotNetPlayground.DefineCli(app);

// Use app.Run(args) to just run once
return app.RunInLoop(args);

