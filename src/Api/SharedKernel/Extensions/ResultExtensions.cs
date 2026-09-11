using Todo.Api.SharedKernel.Models;

namespace Todo.Api.SharedKernel.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(
        this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return Results.Problem(
            statusCode: result.FirstError.StatusCodeValue,
            title: result.FirstError.Code,
            detail: result.FirstError.Description);
    }

    public static IResult ToHttpResult<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return Results.Problem(
            statusCode: result.FirstError.StatusCodeValue,
            title: result.FirstError.Code,
            detail: result.FirstError.Description);
    }

    public static IResult ToHttpResult<T>(
        this PagedResult<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new
            {
                items = result.Items,
                page = result.Page,
                pageSize = result.PageSize,
                totalCount = result.TotalCount,
                totalPages = result.TotalPages,
                hasPreviousPage = result.HasPreviousPage,
                hasNextPage = result.HasNextPage
            });
        }

        return Results.Problem(
            statusCode: result.FirstError.StatusCodeValue,
            title: result.FirstError.Code,
            detail: result.FirstError.Description);
    }
}