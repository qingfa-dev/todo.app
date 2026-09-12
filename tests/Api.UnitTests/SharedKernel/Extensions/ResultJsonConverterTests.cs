using System.Text.Json;

using Shouldly;

using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Extensions;

public class ResultJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new ResultConverterFactory() }
    };

    #region NonGenericResultJsonConverter

    [Fact]
    public void Serialize_ResultSuccess_ShouldWriteIsSuccessTrue()
    {
        var result = Result.Success();

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("\"isSuccess\":true");
        json.ShouldNotContain("errors");
    }

    [Fact]
    public void Serialize_ResultFailure_ShouldWriteIsSuccessFalseAndErrors()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = Result.Failure(error);

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("\"isSuccess\":false");
        json.ShouldContain("errors");
        json.ShouldContain("Test.Code");
    }

    #endregion

    #region ResultJsonConverter<T>

    [Fact]
    public void Serialize_ResultSuccess_WithValue_ShouldSerializeValue()
    {
        var result = Result<string>.Success("hello");

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("hello");
        json.ShouldNotContain("isSuccess");
    }

    [Fact]
    public void Serialize_GenericResultFailure_ShouldWriteIsSuccessFalseAndErrors()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = Result<int>.Failure(error);

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("\"isSuccess\":false");
        json.ShouldContain("errors");
    }

    [Fact]
    public void Serialize_PagedResult_ShouldWritePaginationFields()
    {
        IReadOnlyList<int> items = [1, 2, 3];
        var result = PagedResult<int>.Success(items, 1, 10, 30);

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("\"page\":1");
        json.ShouldContain("\"pageSize\":10");
        json.ShouldContain("\"totalCount\":30");
        json.ShouldContain("\"totalPages\":3");
        json.ShouldContain("\"hasPreviousPage\":false");
        json.ShouldContain("\"hasNextPage\":true");
        json.ShouldContain("items");
    }

    [Fact]
    public void Serialize_PagedResultFailure_ShouldWriteIsSuccessFalse()
    {
        var error = Error.BadRequest("Test.Code", "Test description");
        var result = PagedResult<int>.Failure(error);

        var json = JsonSerializer.Serialize(result, Options);

        json.ShouldContain("\"isSuccess\":false");
        json.ShouldContain("errors");
    }

    #endregion

    #region CanConvert

    [Fact]
    public void CanConvert_WhenResult_ShouldReturnTrue()
    {
        var factory = new ResultConverterFactory();

        factory.CanConvert(typeof(Result)).ShouldBeTrue();
    }

    [Fact]
    public void CanConvert_WhenResultGeneric_ShouldReturnTrue()
    {
        var factory = new ResultConverterFactory();

        factory.CanConvert(typeof(Result<string>)).ShouldBeTrue();
    }

    [Fact]
    public void CanConvert_WhenPagedResult_ShouldReturnTrue()
    {
        var factory = new ResultConverterFactory();

        factory.CanConvert(typeof(PagedResult<int>)).ShouldBeTrue();
    }

    [Fact]
    public void CanConvert_WhenOtherType_ShouldReturnFalse()
    {
        var factory = new ResultConverterFactory();

        factory.CanConvert(typeof(string)).ShouldBeFalse();
    }

    #endregion
}
