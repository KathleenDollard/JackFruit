
using System.ComponentModel.DataAnnotations;

class TodoList
{
    [Required] public string? Title { get; set; }
    public ICollection<Todo>? Todos { get; set; }
}
