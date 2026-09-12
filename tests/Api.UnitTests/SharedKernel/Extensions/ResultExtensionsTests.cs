using System.Net;

using Microsoft.AspNetCore.Http;

using Shouldly;

using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Extensions;

public class ResultExtensionsTests
{
    #region ToHttpResult (Result)

    [Fact]
    public void ToHttpResult_WhenSuccess_ShouldReturnNoContent()
    {
        var result = Result.Success();

        IResult httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    [Fact]
    public void ToHttpResult_WhenFailure_ShouldNotBeSuccess()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = Result.Failure(error);

        IResult httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    #endregion

    #region ToHttpResult (Result<T>)

    [Fact]
    public void ToHttpResult_Generic_WhenSuccess_ShouldReturnOkWithValue()
    {
        var result = Result<string>.Success("hello");

        var httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    [Fact]
    public void ToHttpResult_Generic_WhenFailure_ShouldNotBeSuccess()
    {
        var error = Error.NotFound("Test.Code", "Not found");
        var result = Result<int>.Failure(error);

        var httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    #endregion

    #region ToHttpResult (PagedResult<T>)

    [Fact]
    public void ToHttpResult_Paged_WhenSuccess_ShouldReturnOkWithPagedData()
    {
        IReadOnlyList<string> items = ["a", "b", "c"];
        var result = PagedResult<string>.Success(items, 1, 10, 30);

        var httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    [Fact]
    public void ToHttpResult_Paged_WhenFailure_ShouldNotBeSuccess()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = PagedResult<string>.Failure(error);

        var httpResult = result.ToHttpResult();

        httpResult.ShouldNotBeNull();
    }

    #endregion
}
