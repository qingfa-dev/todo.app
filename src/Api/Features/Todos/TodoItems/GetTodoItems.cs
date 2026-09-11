using System.Linq.Expressions;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Entities;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.Todos.TodoItems;

public static class GetTodoItems
{
    public record Request : TodoItemQueryParameters;

    public record Response : TodoItemItem;

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(
                    1,
                    PagingParameterConstant.Defaults.MaxPageSize);

            RuleFor(x => x.SortDirection)
                .IsInEnum();

            RuleFor(x => x.SortBy)
                .Must(BeSupportedSortField)
                .When(x =>
                    !string.IsNullOrWhiteSpace(x.SortBy));

            RuleFor(x => x.Priority)
                .IsInEnum()
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.Search)
                .MaximumLength(
                    TodoItemConstant.Constraints.TitleMaxLength);

            RuleFor(x => x.Title)
                .MaximumLength(
                    TodoItemConstant.Constraints.TitleMaxLength);

            RuleFor(x => x.Note)
                .MaximumLength(
                    TodoItemConstant.Constraints.NoteMaxLength);
        }

        private static bool BeSupportedSortField(
            string? sortBy)
        {
            return sortBy?.Trim().ToLowerInvariant() switch
            {
                "id" => true,
                "title" => true,
                "note" => true,
                "priority" => true,
                "done" => true,
                "listid" => true,
                _ => false
            };
        }
    }

    public static class Handler
    {
        public static async Task<PagedResult<Response>> Handle(
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
                return PagedResult<Response>.Failure(
                    validationResult
                        .ToErrors()
                        .ToArray());
            }

            IQueryable<TodoItem> query = dbContext
                .Set<TodoItem>()
                .AsNoTracking();

            query = ApplyFilters(
                query,
                request);

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            var sortMapping =
                new Dictionary<string, Expression<Func<TodoItem, object>>>
                {
                    ["id"] = x => x.Id,
                    ["title"] = x => x.Title!,
                    ["note"] = x => x.Note!,
                    ["priority"] = x => x.Priority,
                    ["done"] = x => x.Done,
                    ["listid"] = x => x.ListId
                };

            List<Response> items = await query
                .ApplySorting(
                    request,
                    sortMapping,
                    defaultSort: x => x.Title!)
                .ApplyPaging(request)
                .Select(
                    entity =>
                        TodoItemMapper.ToItem<Response>(
                            entity))
                .ToListAsync(cancellationToken);

            return PagedResult<Response>.Success(
                items,
                request.Page
                    ?? PagingParameterConstant.Defaults.Page,
                request.PageSize
                    ?? PagingParameterConstant.Defaults.PageSize,
                totalCount);
        }

        private static IQueryable<TodoItem> ApplyFilters(
            IQueryable<TodoItem> query,
            Request request)
        {
            if (request.ListId.HasValue)
            {
                query = query.Where(
                    x => x.ListId == request.ListId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    (x.Title != null &&
                     EF.Functions.Like(
                         x.Title,
                         $"%{search}%"))
                    ||
                    (x.Note != null &&
                     EF.Functions.Like(
                         x.Note,
                         $"%{search}%")));
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                var title = request.Title.Trim();

                query = query.Where(x =>
                    x.Title != null &&
                    EF.Functions.Like(
                        x.Title,
                        $"%{title}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Note))
            {
                var note = request.Note.Trim();

                query = query.Where(x =>
                    x.Note != null &&
                    EF.Functions.Like(
                        x.Note,
                        $"%{note}%"));
            }

            if (request.Priority.HasValue)
            {
                query = query.Where(
                    x => x.Priority == request.Priority.Value);
            }

            if (request.Done.HasValue)
            {
                query = query.Where(
                    x => x.Done == request.Done.Value);
            }

            return query;
        }
    }
}
