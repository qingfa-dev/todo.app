namespace Todo.Api.SharedKernel.Models;

public interface IPagedParameters
    : IPagingParameters,
      ISortingParameters
{
}

public record PagedParameters
    : IPagedParameters
{
    public int? Page { get; init; } = PagingParameterConstant.Defaults.Page;
    public int? PageSize { get; init; } = PagingParameterConstant.Defaults.PageSize;

    public string? SortBy { get; init; } = SortingParameterConstant.Defaults.SortBy;

    public SortDirection? SortDirection { get; init; } = SortingParameterConstant.Defaults.SortDirection;
}
