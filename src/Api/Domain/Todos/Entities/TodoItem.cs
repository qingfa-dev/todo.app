using Todo.Api.Domain.Common;
using Todo.Api.Domain.Todos.Enums;

namespace Todo.Api.Domain.Todos.Entities;

public class TodoItem : BaseAuditableEntity
{
    public Guid ListId { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Done { get; set; }
    
    public TodoList List { get; set; } = null!;
}