using System.Net;

using Shouldly;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Models;

public class ErrorTests
{
    #region None

    [Fact]
    public void None_ShouldHaveEmptyCode()
    {
        Error.None.Code.ShouldBe(string.Empty);
    }

    [Fact]
    public void None_ShouldHaveEmptyDescription()
    {
        Error.None.Description.ShouldBe(string.Empty);
    }

    [Fact]
    public void None_ShouldHaveOkStatusCode()
    {
        Error.None.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    #endregion

    #region NullValue

    [Fact]
    public void NullValue_ShouldHaveCorrectProperties()
    {
        Error.NullValue.Code.ShouldBe("General.Null");
        Error.NullValue.Description.ShouldBe("Null value was provided.");
        Error.NullValue.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        var error = new Error("Test.Code", "Test description", HttpStatusCode.BadRequest);

        error.Code.ShouldBe("Test.Code");
        error.Description.ShouldBe("Test description");
        error.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Constructor_WithNullCode_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Error(null!, "Test", HttpStatusCode.BadRequest));
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Error("Test", null!, HttpStatusCode.BadRequest));
    }

    [Fact]
    public void Constructor_WithEmptyCodeAndNonOkStatus_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new Error(string.Empty, "Test", HttpStatusCode.BadRequest));
    }

    [Fact]
    public void Constructor_WithEmptyDescriptionAndNonOkStatus_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new Error("Test", string.Empty, HttpStatusCode.BadRequest));
    }

    [Fact]
    public void Constructor_WithOkStatusCodeAndEmptyCode_ShouldNotThrow()
    {
        var error = new Error(string.Empty, string.Empty, HttpStatusCode.OK);

        error.Code.ShouldBe(string.Empty);
    }

    #endregion

    #region StatusCodeValue

    [Fact]
    public void StatusCodeValue_ShouldReturnIntegerValue()
    {
        var error = Error.BadRequest("Test", "Test");

        error.StatusCodeValue.ShouldBe(400);
    }

    #endregion

    #region Factory methods

    [Fact]
    public void BadRequest_ShouldReturn400StatusCode()
    {
        var error = Error.BadRequest("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Unauthorized_ShouldReturn401StatusCode()
    {
        var error = Error.Unauthorized("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void Forbidden_ShouldReturn403StatusCode()
    {
        var error = Error.Forbidden("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public void NotFound_ShouldReturn404StatusCode()
    {
        var error = Error.NotFound("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public void Conflict_ShouldReturn409StatusCode()
    {
        var error = Error.Conflict("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public void UnprocessableEntity_ShouldReturn422StatusCode()
    {
        var error = Error.UnprocessableEntity("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public void TooManyRequests_ShouldReturn429StatusCode()
    {
        var error = Error.TooManyRequests("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public void Problem_ShouldReturn500StatusCode()
    {
        var error = Error.Problem("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public void ServiceUnavailable_ShouldReturn503StatusCode()
    {
        var error = Error.ServiceUnavailable("Code", "Description");

        error.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    #endregion
}
