namespace Todo.Api.SharedKernel.Models;

public interface IPagingParameters
{
    int? Page { get; }

    int? PageSize { get; }
}

public static class PagingParameterConstant
{
    public static class Defaults
    {
        public const int Page = 1;
        public const int PageSize = 20;
        public const int MaxPageSize = 100;
    }
}
