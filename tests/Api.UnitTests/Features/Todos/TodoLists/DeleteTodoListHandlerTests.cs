using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Features.Todos.TodoLists;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class DeleteTodoListHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly CancellationToken _ct = CancellationToken.None;

    public DeleteTodoListHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();
    private DbSet<TodoItem> Items => _db.Context.Set<TodoItem>();

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFound()
    {
        var request = new DeleteTodoList.Request(Guid.NewGuid());

        var result = await DeleteTodoList.Handler.Handle(
            request, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoListResult.Failure.NotFound(request.Id).Code);
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldReturnSuccess()
    {
        var itemId = SeedList("To Delete");

        var result = await DeleteTodoList.Handler.Handle(
            new DeleteTodoList.Request(itemId), _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldRemoveFromDatabase()
    {
        var itemId = SeedList("To Delete");

        await DeleteTodoList.Handler.Handle(
            new DeleteTodoList.Request(itemId), _db.AppContext, _ct);

        var dbList = Lists.Find(itemId);
        dbList.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldCascadeDeleteItems()
    {
        var listId = SeedList("With Items");

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = "Child Item",
            Priority = PriorityLevel.None,
            Done = false
        };
        Items.Add(item);
        _db.Context.SaveChanges();

        await DeleteTodoList.Handler.Handle(
            new DeleteTodoList.Request(listId), _db.AppContext, _ct);

        var dbItem = Items.Find(item.Id);
        dbItem.ShouldBeNull();
    }

    private Guid SeedList(string title)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = title,
            Colour = Todo.Api.Domain.Todos.ValueObjects.Colour.Grey
        };
        Lists.Add(list);
        _db.Context.SaveChanges();
        return list.Id;
    }
}
