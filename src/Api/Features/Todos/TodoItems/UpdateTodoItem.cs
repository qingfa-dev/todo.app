using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

public static class UpdateTodoItem
{
    public record Request(Guid Id) : TodoItemParameter;

    public record Response : TodoItemDetail;

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            Include(new TodoItemParameterValidator());
        }
    }

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            IApplicationDbContext dbContext,
            IValidator<Request> validator,
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

            TodoItem? entity = await dbContext
                .Set<TodoItem>()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (entity is null)
            {
                return TodoItemResult.Failure
                    .NotFound(request.Id);
            }

            var listExists = await dbContext
                .Set<TodoList>()
                .AnyAsync(
                    x => x.Id == request.ListId,
                    cancellationToken);

            if (!listExists)
            {
                return TodoItemResult.Failure
                    .ListNotFound(request.ListId);
            }

            var title = request.Title!.Trim();

            var duplicate = await dbContext
                .Set<TodoItem>()
                .AnyAsync(
                    x =>
                        x.Id != request.Id &&
                        x.ListId == request.ListId &&
                        x.Title == title,
                    cancellationToken);

            if (duplicate)
            {
                return TodoItemResult.Failure
                    .DuplicateTitle;
            }

            entity.ListId = request.ListId;
            entity.Title = title;
            entity.Note = request.Note?.Trim();
            entity.Priority = request.Priority;
            entity.Done = request.Done;

            await dbContext.SaveChangesAsync(
                cancellationToken);

            return TodoItemMapper.ToDetail<Response>(
                entity);
        }
    }
}