using FluentValidation;
using FluentValidation.Results;

using Shouldly;

using Todo.Api.Features.Todos.TodoLists;

namespace Todo.Api.UnitTests.Features.Todos.TodoLists;

public class GetTodoListsValidatorTests
{
    private readonly GetTodoLists.Validator _validator = new();

    #region Page

    [Fact]
    public void Page_WhenLessThanOne_ShouldHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 0,
            PageSize = 10
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoLists.Request.Page));
    }

    [Fact]
    public void Page_WhenOne_ShouldNotHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.Page));
    }

    #endregion

    #region PageSize

    [Fact]
    public void PageSize_WhenZero_ShouldHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 0
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoLists.Request.PageSize));
    }

    [Fact]
    public void PageSize_WhenExceedsMax_ShouldHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 101
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoLists.Request.PageSize));
    }

    [Fact]
    public void PageSize_WhenAtMax_ShouldNotHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 100
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.PageSize));
    }

    #endregion

    #region SortBy

    [Theory]
    [InlineData("id")]
    [InlineData("title")]
    [InlineData("colour")]
    public void SortBy_WhenSupported_ShouldNotHaveError(string sortBy)
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = sortBy
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.SortBy));
    }

    [Theory]
    [InlineData("unsupported")]
    [InlineData("name")]
    public void SortBy_WhenUnsupported_ShouldHaveError(string sortBy)
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = sortBy
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoLists.Request.SortBy));
    }

    [Fact]
    public void SortBy_WhenNull_ShouldNotHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = null
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.SortBy));
    }

    [Fact]
    public void SortBy_WhenEmpty_ShouldNotHaveError()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = string.Empty
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.SortBy));
    }

    #endregion

    #region Colour

    [Theory]
    [InlineData("#E05C4D")]
    [InlineData("#D98B2B")]
    [InlineData("#4CAF50")]
    public void Colour_WhenSupported_ShouldNotHaveError(string colour)
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            Colour = colour
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.Colour));
    }

    [Theory]
    [InlineData("#000000")]
    [InlineData("invalid")]
    public void Colour_WhenUnsupported_ShouldHaveError(string colour)
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            Colour = colour
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoLists.Request.Colour));
    }

    [Fact]
    public void Colour_ShouldBeCaseInsensitive()
    {
        var request = new GetTodoLists.Request
        {
            Page = 1,
            PageSize = 10,
            Colour = "#e05c4d"
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoLists.Request.Colour));
    }

    #endregion
}
