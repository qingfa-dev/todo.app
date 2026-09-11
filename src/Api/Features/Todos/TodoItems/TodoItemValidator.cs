using FluentValidation;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Results;

namespace Todo.Api.Features.Todos.TodoItems;

public sealed class TodoItemParameterValidator
    : AbstractValidator<TodoItemParameter>
{
    public TodoItemParameterValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty()
            .WithErrorCode(
                TodoItemResult.Failure.ListIdRequired.Code)
            .WithMessage(
                TodoItemResult.Failure.ListIdRequired.Description);

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(
                TodoItemResult.Failure.TitleRequired.Code)
            .WithMessage(
                TodoItemResult.Failure.TitleRequired.Description)
            .MaximumLength(TodoItemConstant.Constraints.TitleMaxLength)
            .WithErrorCode(
                TodoItemResult.Failure.TitleTooLong.Code)
            .WithMessage(
                TodoItemResult.Failure.TitleTooLong.Description);

        RuleFor(x => x.Note)
            .MaximumLength(TodoItemConstant.Constraints.NoteMaxLength)
            .WithErrorCode(
                TodoItemResult.Failure.NoteTooLong.Code)
            .WithMessage(
                TodoItemResult.Failure.NoteTooLong.Description);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithErrorCode(
                TodoItemResult.Failure.PriorityInvalid.Code)
            .WithMessage(
                TodoItemResult.Failure.PriorityInvalid.Description);
    }
}
