namespace Todo.Api.SharedKernel.Models;


public enum SortDirection
{
    Ascending = 0,
    Descending = 1
}

public interface ISortingParameters
{
    string? SortBy { get; }

    SortDirection? SortDirection { get; }
}

public record SortingParameterConstant
{
    public static class Defaults
    {
        public static string SortBy = string.Empty;
        public static SortDirection SortDirection = SortDirection.Ascending;
    }
}

