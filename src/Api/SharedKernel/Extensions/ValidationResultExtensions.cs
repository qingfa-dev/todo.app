using FluentValidation.Results;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.SharedKernel.Extensions;

public static class ValidationResultExtensions
{
    public static Error[] ToErrors(
        this ValidationResult result)
    {
        return [.. result.Errors
            .Select(x => Error.BadRequest(
                x.ErrorCode,
                x.ErrorMessage))];
    }

}