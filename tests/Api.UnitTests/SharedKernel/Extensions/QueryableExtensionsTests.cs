using System.Linq.Expressions;

using Shouldly;

using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Extensions;

public class QueryableExtensionsTests
{
    private static readonly List<int> Source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    private static IQueryable<int> CreateQuery() => Source.AsQueryable();

    #region ApplyPaging

    [Fact]
    public void ApplyPaging_WhenBothPageAndPageSizeProvided_ShouldUseProvidedValues()
    {
        var parameters = new TestPagingParameters { Page = 2, PageSize = 3 };

        var result = CreateQuery().ApplyPaging(parameters).ToList();

        result.ShouldBe([4, 5, 6]);
    }

    [Fact]
    public void ApplyPaging_WhenPageIsNull_ShouldDefaultToPage1()
    {
        var parameters = new TestPagingParameters { Page = null, PageSize = 3 };

        var result = CreateQuery().ApplyPaging(parameters).ToList();

        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void ApplyPaging_WhenPageSizeIsNull_ShouldDefaultToPageSize20()
    {
        var parameters = new TestPagingParameters { Page = 1, PageSize = null };

        var result = CreateQuery().ApplyPaging(parameters).ToList();

        result.Count.ShouldBe(10);
    }

    [Fact]
    public void ApplyPaging_WhenBothAreNull_ShouldUseDefaults()
    {
        var parameters = new TestPagingParameters { Page = null, PageSize = null };

        var result = CreateQuery().ApplyPaging(parameters).ToList();

        result.Count.ShouldBe(10);
    }

    [Fact]
    public void ApplyPaging_WhenPageExceedsTotal_ShouldReturnEmpty()
    {
        var parameters = new TestPagingParameters { Page = 5, PageSize = 3 };

        var result = CreateQuery().ApplyPaging(parameters).ToList();

        result.ShouldBeEmpty();
    }

    #endregion

    #region ApplySorting

    [Fact]
    public void ApplySorting_WhenSortByMatches_AndAscending_ShouldOrderByKey()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "name",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByMatches_AndDescending_ShouldOrderByDescendingKey()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "name",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Descending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["cherry", "banana", "apple"]);
    }

    [Fact]
    public void ApplySorting_WhenSortDirectionIsNull_ShouldDefaultToAscending()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "name",
            SortDirection = null
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByIsNull_ShouldUseDefaultSort()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = null,
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };
        Expression<Func<string, object>> defaultSort = x => x;

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping, defaultSort)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByIsEmpty_ShouldUseDefaultSort()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };
        Expression<Func<string, object>> defaultSort = x => x;

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping, defaultSort)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByIsWhitespace_ShouldUseDefaultSort()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "   ",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };
        Expression<Func<string, object>> defaultSort = x => x;

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping, defaultSort)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByNotInMapping_ShouldUseDefaultSort()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "nonexistent",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };
        Expression<Func<string, object>> defaultSort = x => x;

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping, defaultSort)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByNotInMapping_AndNoDefaultSort_ShouldFallBackToIdentity()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "nonexistent",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByIsCaseInsensitive_ShouldMatchMapping()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "NAME",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    [Fact]
    public void ApplySorting_WhenSortByHasSurroundingWhitespace_ShouldTrimAndMatch()
    {
        var source = new List<string> { "banana", "apple", "cherry" };
        var parameters = new TestSortingParameters
        {
            SortBy = "  name  ",
            SortDirection = Todo.Api.SharedKernel.Models.SortDirection.Ascending
        };
        var mapping = new Dictionary<string, Expression<Func<string, object>>>
        {
            ["name"] = x => x
        };

        var result = source.AsQueryable()
            .ApplySorting(parameters, mapping)
            .ToList();

        result.ShouldBe(["apple", "banana", "cherry"]);
    }

    #endregion

    #region Test helpers

    private sealed class TestPagingParameters : IPagingParameters
    {
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }

    private sealed class TestSortingParameters : ISortingParameters
    {
        public string? SortBy { get; init; }
        public Todo.Api.SharedKernel.Models.SortDirection? SortDirection { get; init; }
    }

    #endregion
}
