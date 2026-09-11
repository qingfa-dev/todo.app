namespace Todo.Api.SharedKernel.Models;

public class PagedResult<T>
    : Result<IReadOnlyList<T>>
{
    private PagedResult(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount)
        : base(
            items,
            true,
            Error.None)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(page));
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize));
        }

        if (totalCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalCount));
        }

        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    private PagedResult(
        params Error[] errors)
        : base(
            default,
            false,
            errors)
    {
        Page = 0;
        PageSize = 0;
        TotalCount = 0;
    }

    public IReadOnlyList<T> Items =>
        IsSuccess
            ? Value
            : [];

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages =>
        PageSize == 0
            ? 0
            : (int)Math.Ceiling(
                TotalCount / (double)PageSize);

    public bool HasPreviousPage =>
        Page > 1;

    public bool HasNextPage =>
        Page < TotalPages;

    public static PagedResult<T> Success(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PagedResult<T>(
            items,
            page,
            pageSize,
            totalCount);
    }

    public new static PagedResult<T> Failure(
        Error error) =>
        new(error);

    public new static PagedResult<T> Failure(
        params Error[] errors) =>
        new(errors);
}
