using FluentValidation;
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

public class UpdateTodoItemHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<UpdateTodoItem.Request> _validator = new UpdateTodoItem.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public UpdateTodoItemHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoItem> Items => _db.Context.Set<TodoItem>();
    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new UpdateTodoItem.Request(Guid.NewGuid())
        {
            ListId = Guid.NewGuid(),
            Title = ""
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotPersistToDatabase()
    {
        var listId = SeedList("List");
        var itemId = SeedItem(listId, "Original");

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = listId,
            Title = ""
        };

        await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        var dbItem = Items.Find(itemId);
        dbItem.ShouldNotBeNull();
        dbItem.Title.ShouldBe("Original");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFound()
    {
        var request = new UpdateTodoItem.Request(Guid.NewGuid())
        {
            ListId = Guid.NewGuid(),
            Title = "Test"
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.NotFound(request.Id).Code);
    }

    [Fact]
    public async Task Handle_WhenListNotFound_ShouldReturnListNotFound()
    {
        var itemId = SeedItem(Guid.NewGuid(), "Item");
        var nonexistentListId = Guid.NewGuid();

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = nonexistentListId,
            Title = "Test"
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.ListNotFound(nonexistentListId).Code);
    }

    [Fact]
    public async Task Handle_WhenDuplicateTitle_ShouldReturnDuplicateTitle()
    {
        var listId = SeedList("List");
        var itemId = SeedItem(listId, "Original");
        SeedItem(listId, "Duplicate");

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = listId,
            Title = "Duplicate"
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.DuplicateTitle.Code);
    }

    [Fact]
    public async Task Handle_WhenUpdatingOwnTitle_ShouldNotConflict()
    {
        var listId = SeedList("List");
        var itemId = SeedItem(listId, "My Title");

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = listId,
            Title = "My Title"
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var listId = SeedList("List");
        var itemId = SeedItem(listId, "Original");

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = listId,
            Title = "Updated Title",
            Note = "Updated Note",
            Priority = PriorityLevel.High,
            Done = true
        };

        var result = await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("Updated Title");
        result.Value.Note.ShouldBe("Updated Note");
        result.Value.Priority.ShouldBe(PriorityLevel.High);
        result.Value.Done.ShouldBe(true);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldPersistToDatabase()
    {
        var listId = SeedList("List");
        var itemId = SeedItem(listId, "Original");

        var request = new UpdateTodoItem.Request(itemId)
        {
            ListId = listId,
            Title = "Persisted"
        };

        await UpdateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        var dbItem = Items.Find(itemId);
        dbItem.ShouldNotBeNull();
        dbItem.Title.ShouldBe("Persisted");
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

    private Guid SeedItem(Guid listId, string title)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = title,
            Priority = PriorityLevel.None,
            Done = false
        };

        Items.Add(item);
        _db.Context.SaveChanges();
        return item.Id;
    }
}
