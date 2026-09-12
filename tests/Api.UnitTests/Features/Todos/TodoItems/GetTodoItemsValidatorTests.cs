using FluentValidation;
using FluentValidation.Results;

using Shouldly;

using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Features.Todos.TodoItems;

namespace Todo.Api.UnitTests.Features.Todos.TodoItems;

public class GetTodoItemsValidatorTests
{
    private readonly GetTodoItems.Validator _validator = new();

    #region Page

    [Fact]
    public void Page_WhenLessThanOne_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 0,
            PageSize = 10
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.Page));
    }

    [Fact]
    public void Page_WhenOne_ShouldNotHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.Page));
    }

    #endregion

    #region PageSize

    [Fact]
    public void PageSize_WhenZero_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 0
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.PageSize));
    }

    [Fact]
    public void PageSize_WhenExceedsMax_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 101
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.PageSize));
    }

    [Fact]
    public void PageSize_WhenAtMax_ShouldNotHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 100
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.PageSize));
    }

    #endregion

    #region SortBy

    [Theory]
    [InlineData("id")]
    [InlineData("title")]
    [InlineData("note")]
    [InlineData("priority")]
    [InlineData("done")]
    [InlineData("listid")]
    public void SortBy_WhenSupported_ShouldNotHaveError(string sortBy)
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = sortBy
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.SortBy));
    }

    [Theory]
    [InlineData("unsupported")]
    [InlineData("name")]
    public void SortBy_WhenUnsupported_ShouldHaveError(string sortBy)
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = sortBy
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.SortBy));
    }

    [Fact]
    public void SortBy_WhenNull_ShouldNotHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = null
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.SortBy));
    }

    [Fact]
    public void SortBy_WhenEmpty_ShouldNotHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            SortBy = string.Empty
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.SortBy));
    }

    #endregion

    #region Priority

    [Theory]
    [InlineData(PriorityLevel.None)]
    [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
    [InlineData(PriorityLevel.High)]
    public void Priority_WhenValid_ShouldNotHaveError(PriorityLevel priority)
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            Priority = priority
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.Priority));
    }

    [Fact]
    public void Priority_WhenNull_ShouldNotHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            Priority = null
        };
        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e => e.PropertyName == nameof(GetTodoItems.Request.Priority));
    }

    #endregion

    #region Search / Title / Note length

    [Fact]
    public void Search_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            Search = new string('a', 101)
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.Search));
    }

    [Fact]
    public void Title_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            Title = new string('a', 101)
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.Title));
    }

    [Fact]
    public void Note_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new GetTodoItems.Request
        {
            Page = 1,
            PageSize = 10,
            Note = new string('a', 501)
        };
        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetTodoItems.Request.Note));
    }

    #endregion
}
