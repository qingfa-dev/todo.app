using Shouldly;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.Features.Todos.TodoLists;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class TodoListValidatorTests
{
    private readonly TodoListParameterValidator _validator = new();

    #region Title

    [Fact]
    public void Title_WhenEmpty_ShouldHaveError()
    {
        var request = new TodoListParameter
        {
            Title = string.Empty,
            Colour = "#E05C4D"
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoListResult.Failure.TitleRequired.Code);
    }

    [Fact]
    public void Title_WhenNull_ShouldHaveError()
    {
        var request = new TodoListParameter
        {
            Title = null,
            Colour = "#E05C4D"
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoListResult.Failure.TitleRequired.Code);
    }

    [Fact]
    public void Title_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new TodoListParameter
        {
            Title = new string('a', TodoListConstant.Constraints.TitleMaxLength + 1),
            Colour = "#E05C4D"
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoListResult.Failure.TitleTooLong.Code);
    }

    [Fact]
    public void Title_WhenAtMaxLength_ShouldNotHaveError()
    {
        var request = new TodoListParameter
        {
            Title = new string('a', TodoListConstant.Constraints.TitleMaxLength),
            Colour = "#E05C4D"
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoListResult.Failure.TitleTooLong.Code);
    }

    #endregion

    #region Colour

    [Fact]
    public void Colour_WhenEmpty_ShouldHaveError()
    {
        var request = new TodoListParameter
        {
            Title = "Test",
            Colour = string.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoListResult.Failure.ColourRequired.Code);
    }

    [Theory]
    [InlineData("#E05C4D")]
    [InlineData("#D98B2B")]
    [InlineData("#4CAF50")]
    [InlineData("#26A69A")]
    [InlineData("#5C6BC0")]
    [InlineData("#AB47BC")]
    [InlineData("#78909C")]
    public void Colour_WhenSupported_ShouldNotHaveError(string colour)
    {
        var request = new TodoListParameter
        {
            Title = "Test",
            Colour = colour
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoListResult.Failure.ColourInvalid.Code);
    }

    [Theory]
    [InlineData("#000000")]
    [InlineData("#FF0000")]
    [InlineData("invalid")]
    public void Colour_WhenUnsupported_ShouldHaveError(string colour)
    {
        var request = new TodoListParameter
        {
            Title = "Test",
            Colour = colour
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoListResult.Failure.ColourInvalid.Code);
    }

    [Fact]
    public void Colour_ShouldBeCaseInsensitive()
    {
        var request = new TodoListParameter
        {
            Title = "Test",
            Colour = "#e05c4d"
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoListResult.Failure.ColourInvalid.Code);
    }

    #endregion

    #region Valid request

    [Fact]
    public void Validate_WhenValidRequest_ShouldBeValid()
    {
        var request = new TodoListParameter
        {
            Title = "Valid Title",
            Colour = "#E05C4D"
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
