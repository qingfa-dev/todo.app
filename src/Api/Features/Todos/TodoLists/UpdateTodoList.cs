using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoLists;

public static class UpdateTodoList
{
    // PUT /api/todo-lists/{id}
    public record Request(Guid Id)
        : TodoListParameter;

    public record Response : TodoListDetail;

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            Include(new TodoListParameterValidator());

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(
                    "Todo list id is required.");
        }
    }

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            IValidator<Request> validator,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            ValidationResult validationResult =
                await validator.ValidateAsync(
                    request,
                    cancellationToken);

            if (!validationResult.IsValid)
            {
                return validationResult
                    .ToErrors()
                    .ToArray();
            }

            TodoList? todoList = await dbContext
                .Set<TodoList>()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (todoList is null)
            {
                return TodoListResult.Failure
                    .NotFound(request.Id);
            }

            var isDuplicate = await dbContext
                .Set<TodoList>()
                .AnyAsync(
                    x =>
                        x.Id != request.Id &&
                        x.Title == request.Title,
                    cancellationToken);

            if (isDuplicate)
            {
                return TodoListResult.Failure
                    .DuplicateTitle;
            }

            todoList.Title = request.Title;
            todoList.Colour =
                Colour.From(request.Colour);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            return TodoListMapper
                .ToDetail<Response>(todoList);
        }
    }
}
