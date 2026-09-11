using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Shouldly;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.Tests.Fixtures;

namespace Todo.Api.Tests.TodoLists;

[Collection("Api")]
public sealed class GetTodoListsTests : ApiTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GetTodoListsTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTodoLists_WhenNoData_ShouldReturnEmptyPagedResult()
    {
        var response = await Client.GetAsync("/api/todo-lists");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(0);
        content.GetProperty("totalCount").GetInt32().ShouldBe(0);
        content.GetProperty("page").GetInt32().ShouldBe(1);
        content.GetProperty("pageSize").GetInt32().ShouldBe(20);
    }

    [Fact]
    public async Task GetTodoLists_WithData_ShouldReturnPagedResults()
    {
        for (int i = 1; i <= 5; i++)
        {
            await SeedTodoList(new TodoList
            {
                Title = $"List {i}",
                Colour = Colour.Grey
            });
        }

        var response = await Client.GetAsync("/api/todo-lists");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(5);
        content.GetProperty("totalCount").GetInt32().ShouldBe(5);
    }

    [Fact]
    public async Task GetTodoLists_WithPageAndSize_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
        {
            await SeedTodoList(new TodoList
            {
                Title = $"List {i:D2}",
                Colour = Colour.Grey
            });
        }

        var response = await Client.GetAsync("/api/todo-lists?page=2&pageSize=3");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(3);
        content.GetProperty("page").GetInt32().ShouldBe(2);
        content.GetProperty("pageSize").GetInt32().ShouldBe(3);
        content.GetProperty("totalCount").GetInt32().ShouldBe(10);
    }

    [Fact]
    public async Task GetTodoLists_WithSortByTitle_ShouldReturnSortedByTitle()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Cherry List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Apple List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Banana List",
            Colour = Colour.Grey
        });

        var response = await Client.GetAsync("/api/todo-lists?sortBy=title&sortDirection=0");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = content.GetProperty("items");
        items.GetArrayLength().ShouldBe(3);
        items[0].GetProperty("title").GetString().ShouldBe("Apple List");
        items[1].GetProperty("title").GetString().ShouldBe("Banana List");
        items[2].GetProperty("title").GetString().ShouldBe("Cherry List");
    }

    [Fact]
    public async Task GetTodoLists_WithSortByTitleDescending_ShouldReturnSortedDesc()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Cherry List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Apple List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Banana List",
            Colour = Colour.Grey
        });

        var response = await Client.GetAsync("/api/todo-lists?sortBy=title&sortDirection=1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = content.GetProperty("items");
        items[0].GetProperty("title").GetString().ShouldBe("Cherry List");
        items[1].GetProperty("title").GetString().ShouldBe("Banana List");
        items[2].GetProperty("title").GetString().ShouldBe("Apple List");
    }

    [Fact]
    public async Task GetTodoLists_WithSortByColour_ShouldReturnSortedByColour()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Grey List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Blue List",
            Colour = Colour.Blue
        });

        await SeedTodoList(new TodoList
        {
            Title = "Red List",
            Colour = Colour.Red
        });

        var response = await Client.GetAsync("/api/todo-lists?sortBy=colour&sortDirection=0");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = content.GetProperty("items");
        items.GetArrayLength().ShouldBe(3);
        items[0].GetProperty("colour").GetString().ShouldBe("#5C6BC0");
        items[1].GetProperty("colour").GetString().ShouldBe("#78909C");
        items[2].GetProperty("colour").GetString().ShouldBe("#E05C4D");
    }

    [Fact]
    public async Task GetTodoLists_WithFilterBySearch_ShouldReturnMatchingItems()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Shopping List",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Work Tasks",
            Colour = Colour.Blue
        });

        await SeedTodoList(new TodoList
        {
            Title = "Shopping Reminders",
            Colour = Colour.Green
        });

        var response = await Client.GetAsync("/api/todo-lists?search=shopping");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task GetTodoLists_WithFilterByTitle_ShouldReturnMatchingItems()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Groceries",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Title = "Work",
            Colour = Colour.Blue
        });

        var response = await Client.GetAsync("/api/todo-lists?title=groceries");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(1);
        content.GetProperty("items")[0].GetProperty("title").GetString().ShouldBe("Groceries");
    }

    [Fact]
    public async Task GetTodoLists_WithFilterByColour_ShouldReturnMatchingItems()
    {
        await SeedTodoList(new TodoList
        {
            Title = "Blue List",
            Colour = Colour.Blue
        });

        await SeedTodoList(new TodoList
        {
            Title = "Red List",
            Colour = Colour.Red
        });

        var response = await Client.GetAsync("/api/todo-lists?colour=%235C6BC0");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(1);
        content.GetProperty("items")[0].GetProperty("title").GetString().ShouldBe("Blue List");
    }

    [Fact]
    public async Task GetTodoLists_WithInvalidPage_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/todo-lists?page=0");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoLists_WithInvalidSortField_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/todo-lists?sortBy=invalidfield");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoLists_WithPaginationMetadata_ShouldReturnCorrectMetadata()
    {
        for (int i = 1; i <= 25; i++)
        {
            await SeedTodoList(new TodoList
            {
                Title = $"List {i:D2}",
                Colour = Colour.Grey
            });
        }

        var response = await Client.GetAsync("/api/todo-lists?page=1&pageSize=10");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("totalCount").GetInt32().ShouldBe(25);
        content.GetProperty("totalPages").GetInt32().ShouldBe(3);
        content.GetProperty("hasPreviousPage").GetBoolean().ShouldBeFalse();
        content.GetProperty("hasNextPage").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task GetTodoLists_WithInvalidColour_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/todo-lists?colour=%23ZZZZZZ");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
