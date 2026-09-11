using Todo.Api.Domain.Todos.Entities;

namespace Todo.Api.Features.Todos.TodoItems;

public static class TodoItemMapper
{
    public static T ToDetail<T>(
        TodoItem entity)
        where T : TodoItemDetail, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new T
        {
            Id = entity.Id,
            ListId = entity.ListId,
            Title = entity.Title,
            Note = entity.Note,
            Priority = entity.Priority,
            Done = entity.Done
        };
    }

    public static T ToItem<T>(
        TodoItem entity)
        where T : TodoItemItem, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new T
        {
            Id = entity.Id,
            ListId = entity.ListId,
            Title = entity.Title,
            Note = entity.Note,
            Priority = entity.Priority,
            Done = entity.Done
        };
    }
}
