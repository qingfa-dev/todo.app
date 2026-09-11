using Microsoft.AspNetCore.Identity;

using Todo.Api.SharedKernel.Models;

namespace Todo.Api.SharedKernel.Extensions;

public static class IdentityResultExtensions
{
    public static Error[] ToErrors(
        this IdentityResult result)
    {
        return [.. result.Errors
            .Select(x => x.Code switch
            {
                "DuplicateEmail" =>
                    Error.Conflict(
                        x.Code,
                        x.Description),

                "DuplicateUserName" =>
                    Error.Conflict(
                        x.Code,
                        x.Description),

                _ =>
                    Error.BadRequest(
                        x.Code,
                        x.Description)
            })];
    }
}
