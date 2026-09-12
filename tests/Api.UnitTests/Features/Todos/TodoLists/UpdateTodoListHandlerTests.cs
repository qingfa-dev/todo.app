using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Features.Todos.TodoLists;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class UpdateTodoListHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<UpdateTodoList.Request> _validator = new UpdateTodoList.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public UpdateTodoListHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new UpdateTodoList.Request(Guid.NewGuid())
        {
            Title = "",
            Colour = "#E05C4D"
        };

        var result = await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotPersistToDatabase()
    {
        var itemId = SeedList("Original");

        var request = new UpdateTodoList.Request(itemId)
        {
            Title = "",
            Colour = "#E05C4D"
        };

        await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        var dbList = Lists.Find(itemId);
        dbList.ShouldNotBeNull();
        dbList.Title.ShouldBe("Original");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFound()
    {
        var request = new UpdateTodoList.Request(Guid.NewGuid())
        {
            Title = "Test",
            Colour = "#E05C4D"
        };

        var result = await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoListResult.Failure.NotFound(request.Id).Code);
    }

    [Fact]
    public async Task Handle_WhenDuplicateTitle_ShouldReturnDuplicateTitle()
    {
        var itemId = SeedList("Original");
        SeedList("Duplicate");

        var request = new UpdateTodoList.Request(itemId)
        {
            Title = "Duplicate",
            Colour = "#E05C4D"
        };

        var result = await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoListResult.Failure.DuplicateTitle.Code);
    }

    [Fact]
    public async Task Handle_WhenUpdatingOwnTitle_ShouldNotConflict()
    {
        var itemId = SeedList("My Title");

        var request = new UpdateTodoList.Request(itemId)
        {
            Title = "My Title",
            Colour = "#E05C4D"
        };

        var result = await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var itemId = SeedList("Original");

        var request = new UpdateTodoList.Request(itemId)
        {
            Title = "Updated Title",
            Colour = "#4CAF50"
        };

        var result = await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("Updated Title");
        result.Value.Colour.ShouldBe("#4CAF50");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldPersistToDatabase()
    {
        var itemId = SeedList("Original");

        var request = new UpdateTodoList.Request(itemId)
        {
            Title = "Persisted",
            Colour = "#E05C4D"
        };

        await UpdateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        var dbList = Lists.Find(itemId);
        dbList.ShouldNotBeNull();
        dbList.Title.ShouldBe("Persisted");
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
