using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.Results;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoLists;

public static class GetTodoList
{
    // GET /api/todo-lists/{id}
    public record Request(Guid Id);

    public record Response : TodoListDetail;

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            TodoList? todoList = await dbContext
                .Set<TodoList>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (todoList is null)
            {
                return TodoListResult.Failure
                    .NotFound(request.Id);
            }

            return TodoListMapper
                .ToDetail<Response>(todoList);
        }
    }
}
