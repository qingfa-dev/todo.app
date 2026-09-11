using Microsoft.EntityFrameworkCore;

namespace Todo.Api.SharedKernel.Data;

public interface IApplicationDbContext
{
    DbSet<TData> Set<TData>()
        where TData : class;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}