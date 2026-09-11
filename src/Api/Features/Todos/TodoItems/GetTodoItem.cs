using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

public static class GetTodoItem
{
    public record Request(Guid Id);

    public record Response : TodoItemDetail;

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            TodoItem? entity = await dbContext
                .Set<TodoItem>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (entity is null)
            {
                return TodoItemResult.Failure
                    .NotFound(request.Id);
            }

            return TodoItemMapper.ToDetail<Response>(entity);
        }
    }
}