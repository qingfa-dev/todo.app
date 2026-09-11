using Carter;

using FluentValidation;

using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

public sealed class TodoItemModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/todo-items")
            .WithTags("Todo Items")
            .RequireAuthorization();

        // =====================================================
        // POST /api/todo-items
        // =====================================================

        group.MapPost(
            "/",
            async (
                CreateTodoItem.Request request,
                IApplicationDbContext dbContext,
                IValidator<CreateTodoItem.Request> validator,
                CancellationToken cancellationToken) =>
            {
                Result<CreateTodoItem.Response> result =
                    await CreateTodoItem.Handler.Handle(
                        request,
                        dbContext,
                        validator,
                        cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // GET /api/todo-items
        // =====================================================

        group.MapGet(
            "/",
            async (
                [AsParameters]
                GetTodoItems.Request request,
                IApplicationDbContext dbContext,
                IValidator<GetTodoItems.Request> validator,
                CancellationToken cancellationToken) =>
            {
                PagedResult<GetTodoItems.Response> result =
                    await GetTodoItems.Handler.Handle(
                        request,
                        dbContext,
                        validator,
                        cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // GET /api/todo-items/{id}
        // =====================================================

        group.MapGet(
            "/{id:guid}",
            async (
                Guid id,
                IApplicationDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var request =
                    new GetTodoItem.Request(id);

                Result<GetTodoItem.Response> result =
                    await GetTodoItem.Handler.Handle(
                        request,
                        dbContext,
                        cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // PUT /api/todo-items/{id}
        // =====================================================

        group.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateTodoItem.Request request,
                IApplicationDbContext dbContext,
                IValidator<UpdateTodoItem.Request> validator,
                CancellationToken cancellationToken) =>
            {
                UpdateTodoItem.Request actualRequest =
                    request with
                    {
                        Id = id
                    };

                Result<UpdateTodoItem.Response> result =
                    await UpdateTodoItem.Handler.Handle(
                        actualRequest,
                        dbContext,
                        validator,
                        cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // DELETE /api/todo-items/{id}
        // =====================================================

        group.MapDelete(
            "/{id:guid}",
            async (
                Guid id,
                IApplicationDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var request =
                    new DeleteTodoItem.Request(id);

                Result result =
                    await DeleteTodoItem.Handler.Handle(
                        request,
                        dbContext,
                        cancellationToken);

                return result.ToHttpResult();
            });
    }
}
