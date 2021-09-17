

namespace JackFruit
{
    // This is from Damian's MinimalApiPlayground
    public class FirstPlayground
    {
        public static void DefineCli(ConsoleApplication app)
        {
            app.AddDescriptions(GetDescriptions());

            //app.Map("todos <id>", (int id, TodoDb db) => Todo.GetTodo(id, db));
            //app.Map("todos --id <identity>", (int id, TodoDb db) => Todo.GetTodo(id, db));
            //app.Map("todos", (int id, TodoDb db) => Todo.GetTodo(id, db));
            //app.Map("todos --x --y", (int id, TodoDb db) => Todo.GetTodo(id, db));
            //app.Map("todos --a --b", (int id, TodoDb db) => Todo.GetTodo(id, db));

            app.Map("", () => "Hello World {something interest}");
            app.Map("hello", () => new { Hello = "World" });
            //app.Map("throw", () => app.Throw(new Exception("uh oh")));
            app.Map("error", () => "An error occurred. This should probably be formatted as Problem Details.");
            app.Map("todos sample", () => new[] {
                    new Todo { Id = 1, Title = "Do this" },
                    new Todo { Id = 2, Title = "Do this too" }
                });
            app.Map("todos <id>", (int id, TodoDb db) => Todo.GetTodo(id, db));



            static Dictionary<string, string> GetDescriptions()
                => new()
                {
                    [""] = "Simple Hello world",
                    ["hello"] = "Fancy Hello world",
                    ["throw"] = "Throw for testing",
                    ["error"] = "Display error, later from handler (may not make sense at Console)",
                    ["todos sample"] = "Display sample data",
                    ["todos"] = "List todos",
                    ["todos incomplete"] = "List things you still need to do",
                    ["todos complete"] = "List things you have done",
                    ["todos {id}"] = "Display details for one to do item"
                };
        }
    }
}
