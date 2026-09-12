using Shouldly;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Enums;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Features.Todos.TodoItems;

namespace Todo.Api.UnitTests.Features.Todos.TodoItems;

public class TodoItemValidatorTests
{
    private readonly TodoItemParameterValidator _validator = new();

    #region ListId

    [Fact]
    public void ListId_WhenEmpty_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.Empty,
            Title = "Test"
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.ListIdRequired.Code);
    }

    [Fact]
    public void ListId_WhenValid_ShouldNotHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test"
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoItemResult.Failure.ListIdRequired.Code);
    }

    #endregion

    #region Title

    [Fact]
    public void Title_WhenEmpty_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = string.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.TitleRequired.Code);
    }

    [Fact]
    public void Title_WhenNull_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = null
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.TitleRequired.Code);
    }

    [Fact]
    public void Title_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = new string('a', TodoItemConstant.Constraints.TitleMaxLength + 1)
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.TitleTooLong.Code);
    }

    [Fact]
    public void Title_WhenAtMaxLength_ShouldNotHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = new string('a', TodoItemConstant.Constraints.TitleMaxLength)
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoItemResult.Failure.TitleTooLong.Code);
    }

    #endregion

    #region Note

    [Fact]
    public void Note_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test",
            Note = new string('a', TodoItemConstant.Constraints.NoteMaxLength + 1)
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.NoteTooLong.Code);
    }

    [Fact]
    public void Note_WhenNull_ShouldNotHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test",
            Note = null
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoItemResult.Failure.NoteTooLong.Code);
    }

    [Fact]
    public void Note_WhenAtMaxLength_ShouldNotHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test",
            Note = new string('a', TodoItemConstant.Constraints.NoteMaxLength)
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoItemResult.Failure.NoteTooLong.Code);
    }

    #endregion

    #region Priority

    [Fact]
    public void Priority_WhenInvalid_ShouldHaveError()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test",
            Priority = (PriorityLevel)999
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == TodoItemResult.Failure.PriorityInvalid.Code);
    }

    [Theory]
    [InlineData(PriorityLevel.None)]
    [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
    [InlineData(PriorityLevel.High)]
    public void Priority_WhenValid_ShouldNotHaveError(PriorityLevel priority)
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Test",
            Priority = priority
        };

        var result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == TodoItemResult.Failure.PriorityInvalid.Code);
    }

    #endregion

    #region Valid request

    [Fact]
    public void Validate_WhenValidRequest_ShouldBeValid()
    {
        var request = new TodoItemParameter
        {
            ListId = Guid.NewGuid(),
            Title = "Valid Title",
            Note = "Valid Note",
            Priority = PriorityLevel.Medium,
            Done = false
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
