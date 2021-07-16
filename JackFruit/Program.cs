using ConsoleSupport;

var builder = ConsoleApplication.CreateBuilder(args);
var app = builder.Build(); // Optional. Can appear anywhere before Run
app.UseExceptionHandler();

app.AddDescriptions(GetDescriptions());

//app.Map("todos <id>", (int id, TodoDb db) => Todo.GetTodo(id, db));
//app.Map("todos --id <identity>", (int id, TodoDb db) => Todo.GetTodo(id, db));
//app.Map("todos", (int id, TodoDb db) => Todo.GetTodo(id, db));
//app.Map("todos --x --y", (int id, TodoDb db) => Todo.GetTodo(id, db));
//app.Map("todos --a --b", (int id, TodoDb db) => Todo.GetTodo(id, db));

app.Map("",() => "Hello World {something interest}");
app.Map("hello", () => new { Hello = "World" });
app.Map("throw", () => throw new Exception("uh oh"));
app.Map("error", () => "An error occurred. This should probably be formatted as Problem Details.");
app.Map("todos sample", () => new[] {
    new Todo { Id = 1, Title = "Do this" },
    new Todo { Id = 2, Title = "Do this too" }
});
app.Map("todos <id>", (int id, TodoDb db) => Todo.GetTodo(id, db));

// You can just run, but this lets you play around. Definitely try help.
while (true)
{
    var input = Console.ReadLine();
    app.Run(input);
}

Dictionary<string, string> GetDescriptions()
    => new()
    {
        ["/"] = "Simple Hello world",
        ["/hello"] = "Fancy Hello world",
        ["/throw"] = "Throw for testing",
        ["/error"] = "Display error, later from handler (may not make sense at Console)",
        ["/todos/sample"] = "Display sample data",
        ["/todos"] = "List todos",
        ["/todos/incomplete"] = "List things you still need to do",
        ["/todos/complete"] = "List things you have done",
        ["/todos/{id}"] = "Display details for one to do item"
    };