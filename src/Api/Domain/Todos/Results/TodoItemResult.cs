using Todo.Api.Domain.Todos.Constants;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Domain.Todos.Results;

public static class TodoItemResult
{
    public static class Failure
    {
        public static Error TitleRequired => Error.BadRequest(
            "TodoItem.Title.Required",
            "Todo item title is required.");

        public static Error TitleTooLong => Error.BadRequest(
            "TodoItem.Title.TooLong",
            $"Todo item title cannot exceed " + $"{TodoItemConstant.Constraints.TitleMaxLength} characters.");

        public static Error NoteTooLong => Error.BadRequest(
            "TodoItem.Note.TooLong",
            $"Todo item note cannot exceed " + $"{TodoItemConstant.Constraints.NoteMaxLength} characters.");

        public static Error ListIdRequired => Error.BadRequest(
            "TodoItem.ListId.Required",
            "Todo list id is required.");

        public static Error ListNotFound(Guid listId) => Error.NotFound(
            "TodoItem.List.NotFound",
            $"Todo list '{listId}' was not found.");

        public static Error PriorityInvalid => Error.BadRequest(
            "TodoItem.Priority.Invalid",
            "Todo item priority is invalid.");

        public static Error NotFound(Guid id) => Error.NotFound(
            "TodoItem.NotFound",
            $"Todo item '{id}' was not found.");

        public static Error DuplicateTitle => Error.Conflict(
            "TodoItem.Title.Duplicate",
            "A todo item with the same title already exists in this list.");

        public static Error AlreadyDeleted => Error.Conflict(
            "TodoItem.AlreadyDeleted",
            "Todo item has already been deleted.");

        public static Error InvalidState => Error.UnprocessableEntity(
            "TodoItem.InvalidState",
            "The todo item is in an invalid state.");
        
        public static Error Forbidden => Error.Forbidden(
            "TodoItem.Forbidden", 
            "You do not have permission to access this todo item.");
    }
}