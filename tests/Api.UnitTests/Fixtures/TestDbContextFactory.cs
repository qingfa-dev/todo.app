using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;

namespace Todo.Api.UnitTests.Fixtures;

public static class TestDbContextFactory
{
    public static (ApplicationDbContext Context, IApplicationDbContext AppContext) Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return (context, context);
    }
}
