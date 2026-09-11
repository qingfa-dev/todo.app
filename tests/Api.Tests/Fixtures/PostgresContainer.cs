using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Npgsql;

namespace Todo.Api.Tests.Fixtures;

public sealed class PostgresContainer : IAsyncLifetime
{
    private const int HostPort = 5433;

    private readonly IContainer _container = new ContainerBuilder("postgres:16-alpine")
        .WithEnvironment("POSTGRES_DB", "todo_test_db")
        .WithEnvironment("POSTGRES_USER", "test_user")
        .WithEnvironment("POSTGRES_PASSWORD", "test_password")
        .WithPortBinding(HostPort, 5432)
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilInternalTcpPortIsAvailable(5432))
        .Build();

    public string ConnectionString =>
        $"Host=localhost;Port={HostPort};Database=todo_test_db;Username=test_user;Password=test_password";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await WaitForReadyAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private async Task WaitForReadyAsync()
    {
        for (int i = 0; i < 30; i++)
        {
            try
            {
                await using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();
                return;
            }
            catch
            {
                await Task.Delay(1000);
            }
        }

        throw new InvalidOperationException(
            "PostgreSQL container did not become ready in time.");
    }
}
