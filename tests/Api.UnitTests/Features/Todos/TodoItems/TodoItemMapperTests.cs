using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Features.Todos.TodoItems;

namespace Todo.Api.UnitTests.Features.Todos.TodoItems;

public class TodoItemMapperTests
{
    #region ToDetail

    [Fact]
    public void ToDetail_ShouldMapAllProperties()
    {
        var entity = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = Guid.NewGuid(),
            Title = "Test Title",
            Note = "Test Note",
            Priority = PriorityLevel.High,
            Done = true
        };

        TodoItemDetail result = TodoItemMapper.ToDetail<TodoItemDetail>(entity);

        result.Id.ShouldBe(entity.Id);
        result.ListId.ShouldBe(entity.ListId);
        result.Title.ShouldBe(entity.Title);
        result.Note.ShouldBe(entity.Note);
        result.Priority.ShouldBe(entity.Priority);
        result.Done.ShouldBe(entity.Done);
    }

    [Fact]
    public void ToDetail_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoItemMapper.ToDetail<TodoItemDetail>(null!));
    }

    [Fact]
    public void ToDetail_WhenTitleIsNull_ShouldMapNull()
    {
        var entity = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = Guid.NewGuid(),
            Title = null,
            Note = null,
            Priority = PriorityLevel.None,
            Done = false
        };

        TodoItemDetail result = TodoItemMapper.ToDetail<TodoItemDetail>(entity);

        result.Title.ShouldBeNull();
        result.Note.ShouldBeNull();
    }

    #endregion

    #region ToItem

    [Fact]
    public void ToItem_ShouldMapAllProperties()
    {
        var entity = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = Guid.NewGuid(),
            Title = "Test Title",
            Note = "Test Note",
            Priority = PriorityLevel.Low,
            Done = false
        };

        TodoItemItem result = TodoItemMapper.ToItem<TodoItemItem>(entity);

        result.Id.ShouldBe(entity.Id);
        result.ListId.ShouldBe(entity.ListId);
        result.Title.ShouldBe(entity.Title);
        result.Note.ShouldBe(entity.Note);
        result.Priority.ShouldBe(entity.Priority);
        result.Done.ShouldBe(entity.Done);
    }

    [Fact]
    public void ToItem_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoItemMapper.ToItem<TodoItemItem>(null!));
    }

    #endregion
}
