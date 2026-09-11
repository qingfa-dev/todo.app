using Todo.Api.Domain.Todos.Enums;

namespace Todo.Api.Domain.Todos.Constants;

public static class TodoItemConstant
{
    public static class Constraints
    {
        public const int TitleMaxLength = 100;
        public const int NoteMaxLength = 500;
    }
    public static class Defaults
    {
        public const string Title = "New Todo Item";
        public const string Note = "This is a new todo item.";
        public const PriorityLevel Priority = PriorityLevel.None;
    }
}