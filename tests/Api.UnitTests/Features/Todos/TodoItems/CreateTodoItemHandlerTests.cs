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

public class CreateTodoItemHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<CreateTodoItem.Request> _validator = new CreateTodoItem.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public CreateTodoItemHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoItem> Items => _db.Context.Set<TodoItem>();
    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new CreateTodoItem.Request
        {
            ListId = Guid.NewGuid(),
            Title = ""
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotPersistToDatabase()
    {
        var listId = SeedList("List");

        var request = new CreateTodoItem.Request
        {
            ListId = listId,
            Title = ""
        };

        await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        Items.Where(x => x.ListId == listId).Count().ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WhenListNotFound_ShouldReturnListNotFound()
    {
        var request = new CreateTodoItem.Request
        {
            ListId = Guid.NewGuid(),
            Title = "Test"
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.ListNotFound(request.ListId).Code);
    }

    [Fact]
    public async Task Handle_WhenDuplicateTitle_ShouldReturnDuplicateTitle()
    {
        var listId = SeedList("List");
        await SeedItem(listId, "Existing Title");

        var request = new CreateTodoItem.Request
        {
            ListId = listId,
            Title = "Existing Title"
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoItemResult.Failure.DuplicateTitle.Code);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var listId = SeedList("List");

        var request = new CreateTodoItem.Request
        {
            ListId = listId,
            Title = "New Title",
            Note = "New Note",
            Priority = PriorityLevel.High,
            Done = false
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("New Title");
        result.Value.Note.ShouldBe("New Note");
        result.Value.Priority.ShouldBe(PriorityLevel.High);
        result.Value.Done.ShouldBe(false);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldTrimTitle()
    {
        var listId = SeedList("List");

        var request = new CreateTodoItem.Request
        {
            ListId = listId,
            Title = "  Trimmed Title  "
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("Trimmed Title");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldPersistToDatabase()
    {
        var listId = SeedList("List");

        var request = new CreateTodoItem.Request
        {
            ListId = listId,
            Title = "Persisted Item"
        };

        var result = await CreateTodoItem.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();

        var dbItem = Items.FirstOrDefault(x => x.Id == result.Value.Id);
        dbItem.ShouldNotBeNull();
        dbItem.Title.ShouldBe("Persisted Item");
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

    private async Task SeedItem(Guid listId, string title)
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
        await _db.Context.SaveChangesAsync();
    }
}
