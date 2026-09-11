using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Shouldly;

using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.Tests.Fixtures;

namespace Todo.Api.Tests.TodoItems;

[Collection("Api")]
public sealed class GetTodoItemsTests : ApiTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GetTodoItemsTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTodoItems_WhenNoData_ShouldReturnEmptyPagedResult()
    {
        var response = await Client.GetAsync("/api/todo-items");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(0);
        content.GetProperty("totalCount").GetInt32().ShouldBe(0);
        content.GetProperty("page").GetInt32().ShouldBe(1);
        content.GetProperty("pageSize").GetInt32().ShouldBe(20);
    }

    [Fact]
    public async Task GetTodoItems_WithData_ShouldReturnPagedResults()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        for (int i = 1; i <= 5; i++)
        {
            await SeedTodoItem(new TodoItem
            {
                ListId = listId,
                Title = $"Item {i:D2}",
                Priority = PriorityLevel.Low,
                Done = false
            });
        }

        var response = await Client.GetAsync("/api/todo-items");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(5);
        content.GetProperty("totalCount").GetInt32().ShouldBe(5);
    }

    [Fact]
    public async Task GetTodoItems_WithPageAndSize_ShouldReturnCorrectPage()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        for (int i = 1; i <= 10; i++)
        {
            await SeedTodoItem(new TodoItem
            {
                ListId = listId,
                Title = $"Item {i:D2}",
                Priority = PriorityLevel.Low,
                Done = false
            });
        }

        var response = await Client.GetAsync("/api/todo-items?page=2&pageSize=3");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(3);
        content.GetProperty("page").GetInt32().ShouldBe(2);
        content.GetProperty("pageSize").GetInt32().ShouldBe(3);
        content.GetProperty("totalCount").GetInt32().ShouldBe(10);
    }

    [Fact]
    public async Task GetTodoItems_WithSortByTitle_ShouldReturnSortedByTitle()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Cherry",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Apple",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Banana",
            Priority = PriorityLevel.Low,
            Done = false
        });

        var response = await Client.GetAsync("/api/todo-items?sortBy=title&sortDirection=0");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = content.GetProperty("items");
        items.GetArrayLength().ShouldBe(3);
        items[0].GetProperty("title").GetString().ShouldBe("Apple");
        items[1].GetProperty("title").GetString().ShouldBe("Banana");
        items[2].GetProperty("title").GetString().ShouldBe("Cherry");
    }

    [Fact]
    public async Task GetTodoItems_WithSortByTitleDescending_ShouldReturnSortedDesc()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Cherry",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Apple",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Banana",
            Priority = PriorityLevel.Low,
            Done = false
        });

        var response = await Client.GetAsync("/api/todo-items?sortBy=title&sortDirection=1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = content.GetProperty("items");
        items[0].GetProperty("title").GetString().ShouldBe("Cherry");
        items[1].GetProperty("title").GetString().ShouldBe("Banana");
        items[2].GetProperty("title").GetString().ShouldBe("Apple");
    }

    [Fact]
    public async Task GetTodoItems_WithFilterByPriority_ShouldReturnMatchingItems()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "High Priority",
            Priority = PriorityLevel.High,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Low Priority",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Another High",
            Priority = PriorityLevel.High,
            Done = false
        });

        var response = await Client.GetAsync("/api/todo-items?priority=3");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(2);
        content.GetProperty("totalCount").GetInt32().ShouldBe(2);
    }

    [Fact]
    public async Task GetTodoItems_WithFilterByDone_ShouldReturnMatchingItems()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Done Item",
            Priority = PriorityLevel.Low,
            Done = true
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Pending Item",
            Priority = PriorityLevel.Low,
            Done = false
        });

        var response = await Client.GetAsync("/api/todo-items?done=true");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(1);
        content.GetProperty("items")[0].GetProperty("title").GetString().ShouldBe("Done Item");
    }

    [Fact]
    public async Task GetTodoItems_WithFilterByListId_ShouldReturnMatchingItems()
    {
        var listId1 = Guid.NewGuid();
        var listId2 = Guid.NewGuid();

        await SeedTodoList(new TodoList
        {
            Id = listId1,
            Title = "List 1",
            Colour = Colour.Grey
        });

        await SeedTodoList(new TodoList
        {
            Id = listId2,
            Title = "List 2",
            Colour = Colour.Blue
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId1,
            Title = "Item in List 1",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId2,
            Title = "Item in List 2",
            Priority = PriorityLevel.Low,
            Done = false
        });

        var response = await Client.GetAsync($"/api/todo-items?listId={listId1}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(1);
        content.GetProperty("items")[0].GetProperty("title").GetString().ShouldBe("Item in List 1");
    }

    [Fact]
    public async Task GetTodoItems_WithSearch_ShouldReturnMatchingItems()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Buy groceries",
            Note = "Milk, eggs, bread",
            Priority = PriorityLevel.Low,
            Done = false
        });

        await SeedTodoItem(new TodoItem
        {
            ListId = listId,
            Title = "Walk the dog",
            Note = "In the park",
            Priority = PriorityLevel.Medium,
            Done = false
        });

        var response = await Client.GetAsync("/api/todo-items?search=groceries");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("items").GetArrayLength().ShouldBe(1);
        content.GetProperty("items")[0].GetProperty("title").GetString().ShouldBe("Buy groceries");
    }

    [Fact]
    public async Task GetTodoItems_WithInvalidPage_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/todo-items?page=0");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoItems_WithInvalidSortField_ShouldReturnValidationError()
    {
        var response = await Client.GetAsync("/api/todo-items?sortBy=invalidfield");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTodoItems_WithPaginationMetadata_ShouldReturnCorrectMetadata()
    {
        var listId = Guid.NewGuid();
        await SeedTodoList(new TodoList
        {
            Id = listId,
            Title = "Test List",
            Colour = Colour.Grey
        });

        for (int i = 1; i <= 25; i++)
        {
            await SeedTodoItem(new TodoItem
            {
                ListId = listId,
                Title = $"Item {i:D2}",
                Priority = PriorityLevel.Low,
                Done = false
            });
        }

        var response = await Client.GetAsync("/api/todo-items?page=1&pageSize=10");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        content.GetProperty("totalCount").GetInt32().ShouldBe(25);
        content.GetProperty("totalPages").GetInt32().ShouldBe(3);
        content.GetProperty("hasPreviousPage").GetBoolean().ShouldBeFalse();
        content.GetProperty("hasNextPage").GetBoolean().ShouldBeTrue();
    }
}
