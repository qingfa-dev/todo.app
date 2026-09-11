using Todo.Api.Domain.Todos.Constants;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Domain.Todos.Results;

public static class TodoListResult
{
    public static class Failure
    {
        // ============================================================
        // Validation
        // ============================================================

        public static Error TitleRequired =>
            Error.BadRequest(
                "TodoList.Title.Required",
                "Todo list title is required.");

        public static Error TitleTooLong =>
            Error.BadRequest(
                "TodoList.Title.TooLong",
                $"Todo list title cannot exceed " +
                $"{TodoListConstant.Constraints.TitleMaxLength} characters.");

        public static Error ColourRequired =>
            Error.BadRequest(
                "TodoList.Colour.Required",
                "Todo list colour is required.");

        public static Error ColourInvalid =>
            Error.BadRequest(
                "TodoList.Colour.Invalid",
                "Todo list colour is invalid.");


        // ============================================================
        // Business
        // ============================================================

        public static Error DuplicateTitle =>
            Error.Conflict(
                "TodoList.Title.Duplicate",
                "A todo list with the same title already exists.");

        public static Error AlreadyDeleted =>
            Error.Conflict(
                "TodoList.AlreadyDeleted",
                "Todo list has already been deleted.");

        public static Error AlreadyArchived =>
            Error.Conflict(
                "TodoList.AlreadyArchived",
                "Todo list has already been archived.");

        public static Error CannotDeleteArchived =>
            Error.Conflict(
                "TodoList.CannotDeleteArchived",
                "An archived todo list cannot be deleted.");

        public static Error InvalidState =>
            Error.UnprocessableEntity(
                "TodoList.InvalidState",
                "The todo list is in an invalid state.");


        // ============================================================
        // Resource
        // ============================================================

        public static Error NotFound(Guid id) =>
            Error.NotFound(
                "TodoList.NotFound",
                $"Todo list '{id}' was not found.");


        // ============================================================
        // Authorization
        // ============================================================

        public static Error Forbidden =>
            Error.Forbidden(
                "TodoList.Forbidden",
                "You do not have permission to access this todo list.");
    }
}
