using ConsoleSupport;
using System.ComponentModel.DataAnnotations;

class Todo
{

    public static Results GetTodo(int id, TodoDb db)
         => db.Todos.Find(id) is Todo todo
              ? Results.Ok(todo)
              : Results.NotFound();

    public int Id { get; set; }
    [Required] public string? Title { get; set; }
    public bool IsComplete { get; set; }
}
