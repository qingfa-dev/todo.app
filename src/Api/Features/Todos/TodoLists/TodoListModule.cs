using Carter;

using FluentValidation;

using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoLists;

public sealed class TodoListModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/todo-lists")
            .WithTags("Todo Lists")
            .RequireAuthorization();

        // =====================================================
        // POST /api/todo-lists
        // =====================================================

        group.MapPost(
            "/",
            async (
                CreateTodoList.Request request,
                IApplicationDbContext dbContext,
                IValidator<CreateTodoList.Request> validator,
                CancellationToken cancellationToken) =>
            {
                Result<CreateTodoList.Response> result =
                    await CreateTodoList.Handler.Handle(
                        request: request,
                        validator: validator,
                        dbContext: dbContext,
                        cancellationToken: cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // GET /api/todo-lists
        // =====================================================

        group.MapGet(
            "/",
            async (
                [AsParameters]
                GetTodoLists.Request request,
                IApplicationDbContext dbContext,
                IValidator<GetTodoLists.Request> validator,
                CancellationToken cancellationToken) =>
            {
                Result<PagedResult<GetTodoLists.Response>> result =
                    await GetTodoLists.Handler.Handle(
                        request: request,
                        validator: validator,
                        dbContext: dbContext,
                        cancellationToken: cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // GET /api/todo-lists/{id}
        // =====================================================

        group.MapGet(
            "/{id:guid}",
            async (
                Guid id,
                IApplicationDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var request =
                    new GetTodoList.Request(id);

                Result<GetTodoList.Response> result =
                    await GetTodoList.Handler.Handle(
                        request,
                        dbContext,
                        cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // PUT /api/todo-lists/{id}
        // =====================================================

        group.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateTodoList.Request request,
                IApplicationDbContext dbContext,
                IValidator<UpdateTodoList.Request> validator,
                CancellationToken cancellationToken) =>
            {
                UpdateTodoList.Request actualRequest =
                    request with
                    {
                        Id = id
                    };

                Result<UpdateTodoList.Response> result =
                    await UpdateTodoList.Handler.Handle(
                        request: actualRequest,
                        validator: validator,
                        dbContext: dbContext,
                        cancellationToken: cancellationToken);

                return result.ToHttpResult();
            });

        // =====================================================
        // DELETE /api/todo-lists/{id}
        // =====================================================

        group.MapDelete(
            "/{id:guid}",
            async (
                Guid id,
                IApplicationDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var request =
                    new DeleteTodoList.Request(id);

                Result result =
                    await DeleteTodoList.Handler.Handle(
                        request,
                        dbContext,
                        cancellationToken);

                return result.ToHttpResult();
            });
    }
}
