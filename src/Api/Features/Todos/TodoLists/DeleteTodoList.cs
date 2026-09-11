using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoLists;

public static class DeleteTodoList
{
    // DELETE /api/todo-lists/{id}
    public record Request(Guid Id);

    public static class Handler
    {
        public static async Task<Result> Handle(
            Request request,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
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

            dbContext
                .Set<TodoList>()
                .Remove(todoList);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            return Result.Success();
        }
    }
}
