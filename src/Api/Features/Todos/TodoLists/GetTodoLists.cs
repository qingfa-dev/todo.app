using System.Linq.Expressions;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.Todos.Entities;
using Todo.Api.Domain.Todos.ValueObjects;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;
using FluentValidation.Results;

namespace Todo.Api.Features.Todos.TodoLists;

public static class GetTodoLists
{
    // GET /api/todo-lists
    public record Request : TodoListQueryParameters;

    public record Response : TodoListItem;

    public sealed class Validator : AbstractValidator<Request>
    {
        private static readonly string[] SupportedSortFields =
        [
            "id",
            "title",
            "colour"
        ];

        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(
                    1,
                    PagingParameterConstant.Defaults.MaxPageSize)
                .WithMessage(
                    $"Page size must be between " +
                    $"1 and " +
                    $"{PagingParameterConstant.Defaults.MaxPageSize}.");

            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("The specified sort direction is invalid.");

            RuleFor(x => x.SortBy)
                .Must(BeSupportedSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage("The specified sort field is not supported.");

            RuleFor(x => x.Colour)
                .Must(BeSupportedColour)
                .When(x => !string.IsNullOrWhiteSpace(x.Colour))
                .WithMessage("The specified colour is invalid.");
        }

        private static bool BeSupportedSortField(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return true;
            }

            return SupportedSortFields.Contains(
                sortBy.Trim(),
                StringComparer.OrdinalIgnoreCase);
        }

        private static bool BeSupportedColour(string? colour)
        {
            if (string.IsNullOrWhiteSpace(colour))
            {
                return true;
            }

            return Colour.SupportedColours.Any(x =>
                string.Equals(
                    x.Code,
                    colour.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class Handler
    {
        public static async Task<Result<PagedResult<Response>>> Handle(
            Request request,
            IValidator<Request> validator,
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await validator.ValidateAsync(
                request,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PagedResult<Response>>.Failure(
                    validationResult
                        .ToErrors());
            }

            IQueryable<TodoList> query = dbContext
                .Set<TodoList>()
                .AsNoTracking();

            query = ApplyFilters(query, request);

            var totalCount = await query.CountAsync(
                cancellationToken);

            var sortMapping =
                new Dictionary<string, Expression<Func<TodoList, object>>>
                {
                    ["id"] = x => x.Id,
                    ["title"] = x => x.Title!,
                    ["colour"] = x => x.Colour.Code
                };

            List<Response> items = await query
                .ApplySorting(
                    request,
                    sortMapping,
                    defaultSort: x => x.Title!)
                .ApplyPaging(request)
                .Select(entity => TodoListMapper.ToItem<Response>(entity))
                .ToListAsync(cancellationToken);

            return PagedResult<Response>.Success(
                items,
                request.Page
                    ?? PagingParameterConstant.Defaults.Page,
                request.PageSize
                    ?? PagingParameterConstant.Defaults.PageSize,
                totalCount);
        }

        private static IQueryable<TodoList> ApplyFilters(
            IQueryable<TodoList> query,
            Request request)
        {
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Title != null &&
                    x.Title.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                var title = request.Title.Trim().ToLower();

                query = query.Where(x =>
                    x.Title != null &&
                    x.Title.ToLower().Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(request.Colour))
            {
                var colour = request.Colour.Trim();

                // Colour codes are validated case-insensitively.
                // Normalize the input to match the stored value.
                Colour supportedColour = Colour.SupportedColours
                    .First(x =>
                        string.Equals(
                            x.Code,
                            colour,
                            StringComparison.OrdinalIgnoreCase));

                query = query.Where(x =>
                    x.Colour.Code == supportedColour.Code);
            }

            return query;
        }
    }
}
