using Todo.Api.Domain.Common;
using Todo.Api.Domain.Todos.ValueObjects;

namespace Todo.Api.Domain.Todos.Entities;

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }
    public Colour Colour { get; set; } = Colour.Grey;
    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}