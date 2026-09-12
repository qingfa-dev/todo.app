using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.Features.Todos.TodoLists;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class TodoListMapperTests
{
    #region ToDetail

    [Fact]
    public void ToDetail_ShouldMapAllProperties()
    {
        var entity = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Colour = Colour.Red
        };

        TodoListDetail result = TodoListMapper.ToDetail<TodoListDetail>(entity);

        result.Id.ShouldBe(entity.Id);
        result.Title.ShouldBe("Test Title");
        result.Colour.ShouldBe("#E05C4D");
    }

    [Fact]
    public void ToDetail_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoListMapper.ToDetail<TodoListDetail>((TodoList)null!));
    }

    #endregion

    #region ToItem

    [Fact]
    public void ToItem_ShouldMapAllProperties()
    {
        var entity = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Colour = Colour.Blue
        };

        TodoListItem result = TodoListMapper.ToItem<TodoListItem>(entity);

        result.Id.ShouldBe(entity.Id);
        result.Title.ShouldBe("Test Title");
        result.Colour.ShouldBe("#5C6BC0");
    }

    [Fact]
    public void ToItem_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoListMapper.ToItem<TodoListItem>((TodoList)null!));
    }

    #endregion

    #region ToDetail (collection)

    [Fact]
    public void ToDetail_Collection_ShouldMapAllEntities()
    {
        var entities = new List<TodoList>
        {
            new() { Id = Guid.NewGuid(), Title = "List 1", Colour = Colour.Red },
            new() { Id = Guid.NewGuid(), Title = "List 2", Colour = Colour.Blue }
        };

        IReadOnlyList<TodoListDetail> result = TodoListMapper.ToDetail<TodoListDetail>(entities);

        result.Count.ShouldBe(2);
        result[0].Title.ShouldBe("List 1");
        result[1].Title.ShouldBe("List 2");
    }

    [Fact]
    public void ToDetail_Collection_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoListMapper.ToDetail<TodoListDetail>((IEnumerable<TodoList>)null!));
    }

    [Fact]
    public void ToDetail_Collection_WhenEmpty_ShouldReturnEmptyList()
    {
        var entities = new List<TodoList>();

        IReadOnlyList<TodoListDetail> result = TodoListMapper.ToDetail<TodoListDetail>(entities);

        result.ShouldBeEmpty();
    }

    #endregion

    #region ToItem (collection)

    [Fact]
    public void ToItem_Collection_ShouldMapAllEntities()
    {
        var entities = new List<TodoList>
        {
            new() { Id = Guid.NewGuid(), Title = "List 1", Colour = Colour.Green },
            new() { Id = Guid.NewGuid(), Title = "List 2", Colour = Colour.Purple }
        };

        IReadOnlyList<TodoListItem> result = TodoListMapper.ToItem<TodoListItem>(entities);

        result.Count.ShouldBe(2);
        result[0].Title.ShouldBe("List 1");
        result[1].Title.ShouldBe("List 2");
    }

    [Fact]
    public void ToItem_Collection_WhenNull_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            TodoListMapper.ToItem<TodoListItem>((IEnumerable<TodoList>)null!));
    }

    #endregion
}
