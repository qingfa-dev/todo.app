using Todo.Api.Domain.Todos.Constants;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoLists;

// Request:
// POST /api/todo-lists
public record TodoListParameter
{
    public string? Title { get; init; } = TodoListConstant.Defaults.Title;

    public string Colour { get; init; } =
        TodoListConstant.Defaults.Colour;
}


// Response base:
public record TodoListResponse : TodoListParameter
{
    public Guid Id { get; init; }
}

// Detail:
// GET /api/todo-lists/{id}
public record TodoListDetail : TodoListResponse
{
}

// List item:
// GET /api/todo-lists
public record TodoListItem : TodoListResponse
{
}

// Parameters:
// GET /api/todo-lists
public record TodoListQueryParameters : PagedParameters
{
    public string? Search { get; init; }
    public string? Title { get; init; }
    public string? Colour { get; init; }
}

