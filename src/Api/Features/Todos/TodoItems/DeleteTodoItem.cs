using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

public static class DeleteTodoItem
{
    public record Request(Guid Id);

    public static class Handler
    {
        public static async Task<Result> Handle(
            Request request,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
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

            dbContext.Set<TodoItem>().Remove(entity);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success();
        }
    }
}
