using Shouldly;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Models;

public class PagedResultTests
{
    #region Success

    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        IReadOnlyList<int> items = [1, 2, 3];

        var result = PagedResult<int>.Success(items, 1, 10, 3);

        result.IsSuccess.ShouldBeTrue();
        result.Items.ShouldBe(items);
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public void Success_ShouldHaveNoneAsFirstError()
    {
        var result = PagedResult<int>.Success([], 1, 10, 0);

        result.FirstError.ShouldBe(Error.None);
    }

    #endregion

    #region Pagination properties

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        var result = PagedResult<int>.Success([], 1, 10, 25);

        result.TotalPages.ShouldBe(3);
    }

    [Fact]
    public void HasPreviousPage_WhenPageIsOne_ShouldBeFalse()
    {
        var result = PagedResult<int>.Success([], 1, 10, 25);

        result.HasPreviousPage.ShouldBeFalse();
    }

    [Fact]
    public void HasPreviousPage_WhenPageIsGreaterThanOne_ShouldBeTrue()
    {
        var result = PagedResult<int>.Success([], 2, 10, 25);

        result.HasPreviousPage.ShouldBeTrue();
    }

    [Fact]
    public void HasNextPage_WhenPageIsLessThanTotalPages_ShouldBeTrue()
    {
        var result = PagedResult<int>.Success([], 1, 10, 25);

        result.HasNextPage.ShouldBeTrue();
    }

    [Fact]
    public void HasNextPage_WhenPageIsEqualToTotalPages_ShouldBeFalse()
    {
        var result = PagedResult<int>.Success([], 3, 10, 25);

        result.HasNextPage.ShouldBeFalse();
    }

    [Fact]
    public void HasNextPage_WhenPageIsGreaterThanTotalPages_ShouldBeFalse()
    {
        var result = PagedResult<int>.Success([], 5, 10, 25);

        result.HasNextPage.ShouldBeFalse();
    }

    #endregion

    #region Items

    [Fact]
    public void Items_WhenFailure_ShouldReturnEmptyList()
    {
        var result = PagedResult<int>.Failure(Error.BadRequest("Test", "Test"));

        result.Items.ShouldBeEmpty();
    }

    #endregion

    #region Constructor validation

    [Fact]
    public void Success_WhenPageIsLessThanOne_ShouldThrowArgumentOutOfRangeException()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            PagedResult<int>.Success([], 0, 10, 25));
    }

    [Fact]
    public void Success_WhenPageSizeIsLessThanOne_ShouldThrowArgumentOutOfRangeException()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            PagedResult<int>.Success([], 1, 0, 25));
    }

    [Fact]
    public void Success_WhenTotalCountIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            PagedResult<int>.Success([], 1, 10, -1));
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_WithSingleError_ShouldReturnFailureResult()
    {
        var error = Error.BadRequest("Test.Code", "Test description");

        var result = PagedResult<int>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnFailureResult()
    {
        var error1 = Error.BadRequest("Code1", "Desc1");
        var error2 = Error.BadRequest("Code2", "Desc2");

        var result = PagedResult<int>.Failure(error1, error2);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Failure_ShouldHaveDefaultPaginationValues()
    {
        var result = PagedResult<int>.Failure(Error.BadRequest("Test", "Test"));

        result.Page.ShouldBe(0);
        result.PageSize.ShouldBe(0);
        result.TotalCount.ShouldBe(0);
    }

    #endregion
}
