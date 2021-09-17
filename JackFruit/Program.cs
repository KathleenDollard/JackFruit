using ConsoleSupport;

var builder = ConsoleApplication.CreateBuilder();
// Add Services here
var app = builder.Build(); 
app.UseExceptionHandler();

// Reasons to not just infer: 
// * Avoid coupling, or recover from conflicts when names do not match
// * Alias
//      `--output|-o`
// * Option argument name differs from option name
//      `--output <outpuPath>`
// * Specifying DI (ideas)
//      Infer from type (at least do this, maybe do another) 
//      `<DI:service`
//      `[service` // conflicts with directives
//      `{serviceName}`            // confusing with interpolated strings
//      attributes on parameter    // would couple methods via delegates and uglify lambdas
//      parameter `di_servicename` // some convention
//      Additional parameters on Map/MapInferred methods

//FirstPlayground.DefineCli(app);
DotNetPlayground.DefineCli(app);

// Use app.Run(args) to just run once
return app.RunInLoop(args);

