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

public class CreateTodoListHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<CreateTodoList.Request> _validator = new CreateTodoList.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public CreateTodoListHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new CreateTodoList.Request
        {
            Title = "",
            Colour = "#E05C4D"
        };

        var result = await CreateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotPersistToDatabase()
    {
        var request = new CreateTodoList.Request
        {
            Title = "",
            Colour = "#E05C4D"
        };

        await CreateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        Lists.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDuplicateTitle_ShouldReturnDuplicateTitle()
    {
        SeedList("Existing");

        var request = new CreateTodoList.Request
        {
            Title = "Existing",
            Colour = "#E05C4D"
        };

        var result = await CreateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == TodoListResult.Failure.DuplicateTitle.Code);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var request = new CreateTodoList.Request
        {
            Title = "New List",
            Colour = "#E05C4D"
        };

        var result = await CreateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("New List");
        result.Value.Colour.ShouldBe("#E05C4D");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldPersistToDatabase()
    {
        var request = new CreateTodoList.Request
        {
            Title = "Persisted List",
            Colour = "#4CAF50"
        };

        var result = await CreateTodoList.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();

        var dbList = Lists.FirstOrDefault(x => x.Id == result.Value.Id);
        dbList.ShouldNotBeNull();
        dbList.Title.ShouldBe("Persisted List");
    }

    private void SeedList(string title)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = title,
            Colour = Todo.Api.Domain.Todos.ValueObjects.Colour.Grey
        };
        Lists.Add(list);
        _db.Context.SaveChanges();
    }
}
