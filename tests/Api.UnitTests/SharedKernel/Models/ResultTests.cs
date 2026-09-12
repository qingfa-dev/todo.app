using System.Net;

using Shouldly;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Models;

public class ResultTests
{
    #region Success

    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Success_ShouldHaveNoneAsFirstError()
    {
        var result = Result.Success();

        result.FirstError.Code.ShouldBe(Error.None.Code);
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_WithSingleError_ShouldReturnFailureResult()
    {
        var error = Error.BadRequest("Test.Code", "Test description");

        var result = Result.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ShouldBe(error);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnFailureResult()
    {
        var error1 = Error.BadRequest("Code1", "Description1");
        var error2 = Error.BadRequest("Code2", "Description2");

        var result = Result.Failure(error1, error2);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Failure_WhenNoErrors_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
        {
            Error[] errors = [];
            var result = Result.Failure(errors);
            return result;
        });
    }

    #endregion

    #region Implicit conversion

    [Fact]
    public void ImplicitConversion_FromError_ShouldReturnFailureResult()
    {
        Error error = Error.BadRequest("Test.Code", "Test description");

        Result result = error;

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    #endregion
}
