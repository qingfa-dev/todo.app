using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

// Used by Create / Update
public record TodoItemParameter
{
    public Guid ListId { get; init; }

    public string? Title { get; init; } =
        TodoItemConstant.Defaults.Title;

    public string? Note { get; init; } =
        TodoItemConstant.Defaults.Note;

    public PriorityLevel Priority { get; init; } =
        TodoItemConstant.Defaults.Priority;

    public bool Done { get; init; }
}

// Common response
public record TodoItemResponse
{
    public Guid Id { get; init; }
    public Guid ListId { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }
    public PriorityLevel Priority { get; init; }
    public bool Done { get; init; }
}

// GET /api/todo-items/{id}
public record TodoItemDetail : TodoItemResponse
{
}

// GET /api/todo-lists/{listId}/todo-items
public record TodoItemItem : TodoItemResponse
{
}

public interface ITodoItemFilterParameters
{
    string? Search { get; }
    string? Title { get; }
    string? Note { get; }
    PriorityLevel? Priority { get; }
    bool? Done { get; }
    Guid? ListId { get; }
}

public record TodoItemQueryParameters
    : PagedParameters,
      ITodoItemFilterParameters
{
    public string? Search { get; init; }

    public string? Title { get; init; }

    public string? Note { get; init; }

    public PriorityLevel? Priority { get; init; }

    public bool? Done { get; init; }

    public Guid? ListId { get; init; }
}
