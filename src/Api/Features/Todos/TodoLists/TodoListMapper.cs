using Todo.Api.Domain.Todos.Entities;

namespace Todo.Api.Features.Todos.TodoLists;

public static class TodoListMapper
{
    public static T ToDetail<T>(TodoList entity)
        where T : TodoListDetail, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new T
        {
            Id = entity.Id,
            Title = entity.Title,
            Colour = entity.Colour.Code
        };
    }

    public static T ToItem<T>(TodoList entity)
        where T : TodoListItem, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new T
        {
            Id = entity.Id,
            Title = entity.Title,
            Colour = entity.Colour.Code
        };
    }

    public static IReadOnlyList<T> ToDetail<T>(
        IEnumerable<TodoList> entities)
        where T : TodoListDetail, new()
    {
        ArgumentNullException.ThrowIfNull(entities);

        return entities
            .Select(ToDetail<T>)
            .ToList();
    }

    public static IReadOnlyList<T> ToItem<T>(
        IEnumerable<TodoList> entities)
        where T : TodoListItem, new()
    {
        ArgumentNullException.ThrowIfNull(entities);

        return entities
            .Select(ToItem<T>)
            .ToList();
    }
}
