using FluentValidation;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Domain.Todos.ValueObjects;

namespace Todo.Api.Features.Todos.TodoLists;

public sealed class TodoListParameterValidator
    : AbstractValidator<TodoListParameter>
{
    public TodoListParameterValidator()
    {
        // ============================================================
        // Title
        // ============================================================

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithErrorCode(TodoListResult.Failure.TitleRequired.Code)
            .WithMessage(TodoListResult.Failure.TitleRequired.Description)

            .MaximumLength(TodoListConstant.Constraints.TitleMaxLength)
            .WithErrorCode(TodoListResult.Failure.TitleTooLong.Code)
            .WithMessage(TodoListResult.Failure.TitleTooLong.Description);


        // ============================================================
        // Colour
        // ============================================================

        RuleFor(x => x.Colour)
            .NotEmpty()
            .WithErrorCode(TodoListResult.Failure.ColourRequired.Code)
            .WithMessage(TodoListResult.Failure.ColourRequired.Description)

            .Must(BeSupportedColour)
            .WithErrorCode(TodoListResult.Failure.ColourInvalid.Code)
            .WithMessage(TodoListResult.Failure.ColourInvalid.Description);
    }

    private static bool BeSupportedColour(string colour)
    {
        return Colour.SupportedColours
            .Any(x => string.Equals(
                x.Code,
                colour,
                StringComparison.OrdinalIgnoreCase));
    }
}
