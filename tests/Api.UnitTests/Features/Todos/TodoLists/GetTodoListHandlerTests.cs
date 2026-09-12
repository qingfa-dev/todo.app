using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Features.Todos.TodoLists;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class GetTodoListHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GetTodoListHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFound()
    {
        var request = new GetTodoList.Request(Guid.NewGuid());

        var result = await GetTodoList.Handler.Handle(
            request, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == Todo.Api.Domain.Todos.Results.TodoListResult.Failure.NotFound(request.Id).Code);
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldReturnSuccess()
    {
        var itemId = SeedList("Test Title", Todo.Api.Domain.Todos.ValueObjects.Colour.Blue);

        var request = new GetTodoList.Request(itemId);

        var result = await GetTodoList.Handler.Handle(
            request, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(itemId);
        result.Value.Title.ShouldBe("Test Title");
        result.Value.Colour.ShouldBe("#5C6BC0");
    }

    private Guid SeedList(string title, Todo.Api.Domain.Todos.ValueObjects.Colour colour)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = title,
            Colour = colour
        };
        Lists.Add(list);
        _db.Context.SaveChanges();
        return list.Id;
    }
}
