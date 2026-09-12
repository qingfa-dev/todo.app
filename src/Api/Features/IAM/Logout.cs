using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM.Services;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.IAM;

public static class Logout
{
    public record Request
    {
        public string RefreshToken { get; init; } =
            string.Empty;
    }

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithErrorCode(
                    IAMResult
                        .Failure
                        .RefreshTokenRequired
                        .Code)
                .WithMessage(
                    IAMResult
                        .Failure
                        .RefreshTokenRequired
                        .Description);
        }
    }

    public static class Handler
    {
        public static async Task<Result> Handle(
            Request request,
            IApplicationDbContext dbContext,
            IJwtTokenService tokenService,
            IValidator<Request> validator,
            CancellationToken cancellationToken)
        {
            ValidationResult validationResult =
                await validator.ValidateAsync(
                    request,
                    cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure(
                    validationResult
                        .ToErrors()
                        .ToArray());
            }

            var tokenHash =
                tokenService.HashRefreshToken(
                    request.RefreshToken);

            RefreshToken? storedToken =
                await dbContext
                    .Set<RefreshToken>()
                    .SingleOrDefaultAsync(
                        x => x.TokenHash == tokenHash,
                        cancellationToken);

            // Logout is intentionally idempotent.
            // If the token doesn't exist, there is
            // nothing left to revoke.
            if (storedToken is null)
            {
                return Result.Success();
            }

            if (!storedToken.IsRevoked)
            {
                storedToken.RevokedAtUtc =
                    DateTime.UtcNow;

                await dbContext.SaveChangesAsync(
                    cancellationToken);
            }

            return Result.Success();
        }
    }
}