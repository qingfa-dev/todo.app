using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Models;
using Todo.Api.SharedKernel.Extensions;
using FluentValidation.Results;

namespace Todo.Api.Features.Todos.TodoLists;

public static class CreateTodoList
{
    // POST /api/todo-lists
    public record Request : TodoListParameter;

    public record Response : TodoListDetail;

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            Include(new TodoListParameterValidator());
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
                return validationResult.ToErrors();

            var isDuplicate = await dbContext
                .Set<TodoList>()
                .AnyAsync(
                    x => x.Title == request.Title,
                    cancellationToken);

            if (isDuplicate)
            {
                return TodoListResult.Failure.DuplicateTitle;
            }

            var todoList = new TodoList
            {
                Title = request.Title,
                Colour = Colour.From(request.Colour)
            };

            dbContext
                .Set<TodoList>()
                .Add(todoList);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            return TodoListMapper.ToDetail<Response>(
                todoList);
        }
    }
}
