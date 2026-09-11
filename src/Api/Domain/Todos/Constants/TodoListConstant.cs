using Todo.Api.Domain.Todos.Enums;

namespace Todo.Api.Domain.Todos.Constants;

public static class TodoListConstant
{
    public static class Constraints
    {
        public const int TitleMaxLength = 100;
        public const int ColorMaxLength = 7;
    }

    public static class Defaults
    {
        public const string Title = "New Todo List";
        public const string Colour = "#FFFFFF";
        public const PriorityLevel PriorityLevel = PriorityLevel.Medium;
    }
}
