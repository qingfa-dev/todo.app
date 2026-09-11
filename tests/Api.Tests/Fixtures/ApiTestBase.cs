using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;

namespace Todo.Api.Tests.Fixtures;

[Collection("Api")]
public abstract class ApiTestBase : IAsyncLifetime
{
    private readonly ApiFactory _factory;
    private IServiceScope _scope = null!;

    protected HttpClient Client { get; private set; } = null!;
    protected IApplicationDbContext DbContext { get; private set; } = null!;
    protected ApplicationDbContext RawDbContext { get; private set; } = null!;

    protected ApiTestBase(ApiFactory factory)
    {
        _factory = factory;
    }

    public virtual async Task InitializeAsync()
    {
        Client = _factory.CreateClient();

        var scope = _factory.Services.CreateScope();
        _scope = scope;

        RawDbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        DbContext = scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        await RawDbContext.Database.MigrateAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await RawDbContext.Database.EnsureDeletedAsync();

        _scope.Dispose();
        Client.Dispose();

        await Task.CompletedTask;
    }

    protected async Task SeedTodoList(
        Domain.Todos.Entities.TodoList list)
    {
        DbContext.Set<Domain.Todos.Entities.TodoList>().Add(list);
        await DbContext.SaveChangesAsync();
    }

    protected async Task SeedTodoItem(
        Domain.Todos.Entities.TodoItem item)
    {
        DbContext.Set<Domain.Todos.Entities.TodoItem>().Add(item);
        await DbContext.SaveChangesAsync();
    }
}
