using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Features.Todos.TodoLists;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class GetTodoListsHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<GetTodoLists.Request> _validator = new GetTodoLists.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public GetTodoListsHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new GetTodoLists.Request
        {
            Page = 0
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenNoLists_ShouldReturnEmptyPage()
    {
        var request = new GetTodoLists.Request();

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WhenListsExist_ShouldReturnAll()
    {
        SeedList("List A");
        SeedList("List B");
        SeedList("List C");

        var request = new GetTodoLists.Request
        {
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(3);
        result.Value.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task Handle_WhenSearchFilter_ShouldMatchTitle()
    {
        SeedList("Groceries");
        SeedList("Work Tasks");
        SeedList("Shopping");

        var request = new GetTodoLists.Request
        {
            Search = "shop",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items[0].Title.ShouldBe("Shopping");
    }

    [Fact]
    public async Task Handle_WhenTitleFilter_ShouldMatchTitle()
    {
        SeedList("Groceries");
        SeedList("Work Tasks");
        SeedList("Grocery List");

        var request = new GetTodoLists.Request
        {
            Title = "grocer",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenColourFilter_ShouldMatchColour()
    {
        SeedListWithColour("Red List", Todo.Api.Domain.Todos.ValueObjects.Colour.Red);
        SeedListWithColour("Blue List", Todo.Api.Domain.Todos.ValueObjects.Colour.Blue);
        SeedListWithColour("Another Red", Todo.Api.Domain.Todos.ValueObjects.Colour.Red);

        var request = new GetTodoLists.Request
        {
            Colour = "#E05C4D",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Items.ShouldAllBe(x => x.Colour == "#E05C4D");
    }

    [Fact]
    public async Task Handle_WhenCombinedFilters_ShouldApplyAll()
    {
        SeedListWithColour("Red groceries", Todo.Api.Domain.Todos.ValueObjects.Colour.Red);
        SeedListWithColour("Blue groceries", Todo.Api.Domain.Todos.ValueObjects.Colour.Blue);
        SeedListWithColour("Red work", Todo.Api.Domain.Todos.ValueObjects.Colour.Red);

        var request = new GetTodoLists.Request
        {
            Search = "groceries",
            Colour = "#E05C4D",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items[0].Title.ShouldBe("Red groceries");
    }

    [Fact]
    public async Task Handle_WhenNoMatchFilter_ShouldReturnEmpty()
    {
        SeedList("List A");

        var request = new GetTodoLists.Request
        {
            Search = "nonexistent",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WhenPaging_ShouldReturnCorrectPage()
    {
        for (var i = 1; i <= 5; i++)
        {
            SeedList($"List {i:D2}");
        }

        var request = new GetTodoLists.Request
        {
            Page = 2,
            PageSize = 2,
            SortBy = "title",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Page.ShouldBe(2);
        result.Value.PageSize.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(5);
        result.Value.TotalPages.ShouldBe(3);
        result.Value.HasPreviousPage.ShouldBeTrue();
        result.Value.HasNextPage.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenSortByTitleAscending_ShouldReturnSorted()
    {
        SeedList("Charlie");
        SeedList("Alpha");
        SeedList("Bravo");

        var request = new GetTodoLists.Request
        {
            SortBy = "title",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending,
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(3);
        result.Value.Items[0].Title.ShouldBe("Alpha");
        result.Value.Items[1].Title.ShouldBe("Bravo");
        result.Value.Items[2].Title.ShouldBe("Charlie");
    }

    [Fact]
    public async Task Handle_WhenSortByTitleDescending_ShouldReturnSorted()
    {
        SeedList("Charlie");
        SeedList("Alpha");
        SeedList("Bravo");

        var request = new GetTodoLists.Request
        {
            SortBy = "title",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Descending,
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items[0].Title.ShouldBe("Charlie");
        result.Value.Items[1].Title.ShouldBe("Bravo");
        result.Value.Items[2].Title.ShouldBe("Alpha");
    }

    [Fact]
    public async Task Handle_WhenDefaultSort_ShouldSortByTitle()
    {
        SeedList("Charlie");
        SeedList("Alpha");
        SeedList("Bravo");

        var request = new GetTodoLists.Request
        {
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items[0].Title.ShouldBe("Alpha");
        result.Value.Items[1].Title.ShouldBe("Bravo");
        result.Value.Items[2].Title.ShouldBe("Charlie");
    }

    [Fact]
    public async Task Handle_WhenPagingExceedsTotal_ShouldReturnEmptyPage()
    {
        SeedList("Only List");

        var request = new GetTodoLists.Request
        {
            Page = 10,
            PageSize = 5
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WhenSearchIsCaseInsensitive_ShouldMatch()
    {
        SeedList("GROCERIES");

        var request = new GetTodoLists.Request
        {
            Search = "groceries",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WhenColourFilterCaseInsensitive_ShouldMatch()
    {
        SeedListWithColour("Red List", Todo.Api.Domain.Todos.ValueObjects.Colour.Red);

        var request = new GetTodoLists.Request
        {
            Colour = "#e05c4d",
            PageSize = 10
        };

        var result = await GetTodoLists.Handler.Handle(
            request, _validator, _db.AppContext, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
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

    private void SeedListWithColour(string title, Todo.Api.Domain.Todos.ValueObjects.Colour colour)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = title,
            Colour = colour
        };
        Lists.Add(list);
        _db.Context.SaveChanges();
    }
}
