using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Features.Todos.TodoItems;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoItems;

public class GetTodoItemHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GetTodoItemHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoItem> Items => _db.Context.Set<TodoItem>();
    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFound()
    {
        var request = new GetTodoItem.Request(Guid.NewGuid());

        var result = await GetTodoItem.Handler.Handle(
            request, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.NotFound(request.Id).Code);
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldReturnSuccess()
    {
        var itemId = SeedItem("Test Title", "Test Note", PriorityLevel.High, true);

        var request = new GetTodoItem.Request(itemId);

        var result = await GetTodoItem.Handler.Handle(
            request, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(itemId);
        result.Value.Title.ShouldBe("Test Title");
        result.Value.Note.ShouldBe("Test Note");
        result.Value.Priority.ShouldBe(PriorityLevel.High);
        result.Value.Done.ShouldBe(true);
    }

    private Guid SeedItem(string title, string note, PriorityLevel priority, bool done)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "List",
            Colour = Todo.Api.Domain.Todos.ValueObjects.Colour.Grey
        };
        Lists.Add(list);

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = list.Id,
            Title = title,
            Note = note,
            Priority = priority,
            Done = done
        };
        Items.Add(item);
        _db.Context.SaveChanges();
        return item.Id;
    }
}
