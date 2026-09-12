using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Features.Todos.TodoItems;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.Todos.TodoItems;

public class GetTodoItemsHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly IValidator<GetTodoItems.Request> _validator = new GetTodoItems.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public GetTodoItemsHandlerTests() => _db = TestDbContextFactory.Create();

    public void Dispose() => _db.Context.Dispose();

    private DbSet<TodoItem> Items => _db.Context.Set<TodoItem>();
    private DbSet<TodoList> Lists => _db.Context.Set<TodoList>();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new GetTodoItems.Request
        {
            Page = 0
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenNoItems_ShouldReturnEmptyPage()
    {
        var request = new GetTodoItems.Request();

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WhenItemsExist_ShouldReturnAll()
    {
        var listId = SeedList("List");
        SeedItem(listId, "Item A");
        SeedItem(listId, "Item B");
        SeedItem(listId, "Item C");

        var request = new GetTodoItems.Request
        {
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task Handle_WhenListIdFilter_ShouldReturnOnlyItemsForList()
    {
        var listA = SeedList("List A");
        var listB = SeedList("List B");
        SeedItem(listA, "Item A1");
        SeedItem(listA, "Item A2");
        SeedItem(listB, "Item B1");

        var request = new GetTodoItems.Request
        {
            ListId = listA,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(x => x.ListId == listA);
    }

    [Fact]
    public async Task Handle_WhenSearchFilter_ShouldMatchTitleOrNote()
    {
        var listId = SeedList("List");
        SeedItemWithNote(listId, "Groceries", "Buy milk");
        SeedItemWithNote(listId, "Work", "Finish report");
        SeedItemWithNote(listId, "Shopping", "Buy clothes");

        var request = new GetTodoItems.Request
        {
            Search = "buy",
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenTitleFilter_ShouldMatchTitle()
    {
        var listId = SeedList("List");
        SeedItem(listId, "Groceries");
        SeedItem(listId, "Work Tasks");
        SeedItem(listId, "Grocery List");

        var request = new GetTodoItems.Request
        {
            Title = "grocer",
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenNoteFilter_ShouldMatchNote()
    {
        var listId = SeedList("List");
        SeedItemWithNote(listId, "Item 1", "Buy milk and eggs");
        SeedItemWithNote(listId, "Item 2", "Finish report");
        SeedItemWithNote(listId, "Item 3", "Buy groceries");

        var request = new GetTodoItems.Request
        {
            Note = "buy",
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenPriorityFilter_ShouldMatchPriority()
    {
        var listId = SeedList("List");
        SeedItemWithPriority(listId, "None Item", PriorityLevel.None);
        SeedItemWithPriority(listId, "High Item", PriorityLevel.High);
        SeedItemWithPriority(listId, "Another High", PriorityLevel.High);

        var request = new GetTodoItems.Request
        {
            Priority = PriorityLevel.High,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(x => x.Priority == PriorityLevel.High);
    }

    [Fact]
    public async Task Handle_WhenDoneFilter_ShouldMatchDoneStatus()
    {
        var listId = SeedList("List");
        SeedItemWithDone(listId, "Done Item", true);
        SeedItemWithDone(listId, "Not Done", false);
        SeedItemWithDone(listId, "Also Done", true);

        var request = new GetTodoItems.Request
        {
            Done = true,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldAllBe(x => x.Done);
    }

    [Fact]
    public async Task Handle_WhenCombinedFilters_ShouldApplyAll()
    {
        var listA = SeedList("List A");
        var listB = SeedList("List B");
        SeedItemWithPriority(listA, "High Work", PriorityLevel.High);
        SeedItemWithPriority(listA, "Low Work", PriorityLevel.Low);
        SeedItemWithPriority(listB, "High Other", PriorityLevel.High);

        var request = new GetTodoItems.Request
        {
            ListId = listA,
            Priority = PriorityLevel.High,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(1);
        result.Items[0].Title.ShouldBe("High Work");
    }

    [Fact]
    public async Task Handle_WhenNoMatchFilter_ShouldReturnEmpty()
    {
        var listId = SeedList("List");
        SeedItem(listId, "Item");

        var request = new GetTodoItems.Request
        {
            Search = "nonexistent",
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WhenPaging_ShouldReturnCorrectPage()
    {
        var listId = SeedList("List");
        for (var i = 1; i <= 5; i++)
        {
            SeedItem(listId, $"Item {i:D2}");
        }

        var request = new GetTodoItems.Request
        {
            Page = 2,
            PageSize = 2,
            SortBy = "title",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(2);
        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(2);
        result.TotalCount.ShouldBe(5);
        result.TotalPages.ShouldBe(3);
        result.HasPreviousPage.ShouldBeTrue();
        result.HasNextPage.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenSortByTitleAscending_ShouldReturnSorted()
    {
        var listId = SeedList("List");
        SeedItem(listId, "Charlie");
        SeedItem(listId, "Alpha");
        SeedItem(listId, "Bravo");

        var request = new GetTodoItems.Request
        {
            SortBy = "title",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(3);
        result.Items[0].Title.ShouldBe("Alpha");
        result.Items[1].Title.ShouldBe("Bravo");
        result.Items[2].Title.ShouldBe("Charlie");
    }

    [Fact]
    public async Task Handle_WhenSortByPriorityDescending_ShouldReturnSorted()
    {
        var listId = SeedList("List");
        SeedItemWithPriority(listId, "None", PriorityLevel.None);
        SeedItemWithPriority(listId, "High", PriorityLevel.High);
        SeedItemWithPriority(listId, "Medium", PriorityLevel.Medium);

        var request = new GetTodoItems.Request
        {
            SortBy = "priority",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Descending,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items[0].Priority.ShouldBe(PriorityLevel.High);
        result.Items[1].Priority.ShouldBe(PriorityLevel.Medium);
        result.Items[2].Priority.ShouldBe(PriorityLevel.None);
    }

    [Fact]
    public async Task Handle_WhenSortByDoneAscending_ShouldReturnSorted()
    {
        var listId = SeedList("List");
        SeedItemWithDone(listId, "Done", true);
        SeedItemWithDone(listId, "Not Done", false);

        var request = new GetTodoItems.Request
        {
            SortBy = "done",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items[0].Done.ShouldBeFalse();
        result.Items[1].Done.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenPagingExceedsTotal_ShouldReturnEmptyPage()
    {
        var listId = SeedList("List");
        SeedItem(listId, "Only Item");

        var request = new GetTodoItems.Request
        {
            Page = 10,
            PageSize = 5
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WhenSearchInNote_ShouldMatch()
    {
        var listId = SeedList("List");
        SeedItemWithNote(listId, "Item 1", "Buy milk");
        SeedItemWithNote(listId, "Item 2", "Sell car");

        var request = new GetTodoItems.Request
        {
            Search = "sell",
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(1);
        result.Items[0].Title.ShouldBe("Item 2");
    }

    [Fact]
    public async Task Handle_WhenMultipleLists_ShouldFilterCorrectly()
    {
        var listA = SeedList("List A");
        var listB = SeedList("List B");
        var listC = SeedList("List C");
        SeedItem(listA, "A Item");
        SeedItem(listB, "B Item");
        SeedItem(listC, "C Item");

        var request = new GetTodoItems.Request
        {
            ListId = listB,
            PageSize = 10
        };

        var result = await GetTodoItems.Handler.Handle(
            request, _db.AppContext, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Items.Count.ShouldBe(1);
        result.Items[0].Title.ShouldBe("B Item");
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

    private void SeedItem(Guid listId, string title)
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
    }

    private void SeedItemWithNote(Guid listId, string title, string note)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = title,
            Note = note,
            Priority = PriorityLevel.None,
            Done = false
        };
        Items.Add(item);
        _db.Context.SaveChanges();
    }

    private void SeedItemWithPriority(Guid listId, string title, PriorityLevel priority)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = title,
            Priority = priority,
            Done = false
        };
        Items.Add(item);
        _db.Context.SaveChanges();
    }

    private void SeedItemWithDone(Guid listId, string title, bool done)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = title,
            Priority = PriorityLevel.None,
            Done = done
        };
        Items.Add(item);
        _db.Context.SaveChanges();
    }
}
