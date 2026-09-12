using System.Net;

using Shouldly;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Models;

public class ResultGenericTests
{
    #region Success

    [Fact]
    public void Success_ShouldReturnSuccessResultWithValue()
    {
        var result = Result<string>.Success("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void Success_ShouldHaveNoneAsFirstError()
    {
        var result = Result<int>.Success(42);

        result.FirstError.ShouldBe(Error.None);
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_WithSingleError_ShouldReturnFailureResult()
    {
        var error = Error.BadRequest("Test.Code", "Test description");

        var result = Result<int>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnFailureResult()
    {
        var error1 = Error.BadRequest("Code1", "Desc1");
        var error2 = Error.BadRequest("Code2", "Desc2");

        var result = Result<int>.Failure(error1, error2);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Value_WhenFailure_ShouldThrowInvalidOperationException()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = Result<int>.Failure(error);

        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    #endregion

    #region ValidationFailure

    [Fact]
    public void ValidationFailure_ShouldReturnFailureResult()
    {
        var error = Error.BadRequest("Validation.Error", "Validation failed");

        var result = Result<int>.ValidationFailure(error);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    #endregion

    #region Implicit conversion

    [Fact]
    public void ImplicitConversion_FromValue_ShouldReturnSuccessResult()
    {
        Result<string> result = "hello";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void ImplicitConversion_FromNull_ShouldReturnFailureResult()
    {
        string? value = null;

        Result<string> result = value;

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldReturnFailureResult()
    {
        Result<int> result = Error.BadRequest("Test.Code", "Test description");

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromErrorArray_ShouldReturnFailureResult()
    {
        Error[] errors =
        [
            Error.BadRequest("Code1", "Desc1"),
            Error.BadRequest("Code2", "Desc2")
        ];

        Result<int> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void ImplicitConversion_FromErrorList_ShouldReturnFailureResult()
    {
        List<Error> errors =
        [
            Error.BadRequest("Code1", "Desc1"),
            Error.BadRequest("Code2", "Desc2")
        ];

        Result<int> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }

    #endregion
}
