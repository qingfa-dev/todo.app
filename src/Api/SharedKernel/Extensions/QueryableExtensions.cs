using System.Linq.Expressions;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.SharedKernel.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPaging<T>(
        this IQueryable<T> query,
        IPagingParameters parameters)
    {
        var page = parameters.Page
            ?? PagingParameterConstant.Defaults.Page;
        var pageSize = parameters.PageSize
            ?? PagingParameterConstant.Defaults.PageSize;

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        ISortingParameters parameters,
        IDictionary<string, Expression<Func<T, object>>> sortMapping,
        Expression<Func<T, object>>? defaultSort = null)
    {
        var sortBy = parameters.SortBy?
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(sortBy) ||
            !sortMapping.TryGetValue(sortBy, out Expression<Func<T, object>>? keySelector))
        {
            keySelector = defaultSort ?? (x => x!);
        }

        var descending =
            parameters.SortDirection == SortDirection.Descending;

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
