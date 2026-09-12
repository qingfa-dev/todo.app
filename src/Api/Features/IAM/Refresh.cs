using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Features.IAM.Services;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.IAM;

public static class Refresh
{
    public record Request
    {
        public string RefreshToken { get; init; } =
            string.Empty;
    }

    public record Response : AuthResponse;

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
        public static async Task<Result<Response>> Handle(
            Request request,
            IApplicationDbContext dbContext,
            IJwtTokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions,
            IValidator<Request> validator,
            CancellationToken cancellationToken)
        {
            ValidationResult validationResult =
                await validator.ValidateAsync(
                    request,
                    cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<Response>.Failure(
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

            if (storedToken is null)
            {
                return IAMResult
                    .Failure
                    .RefreshTokenInvalid;
            }

            if (storedToken.IsRevoked)
            {
                return IAMResult
                    .Failure
                    .RefreshTokenRevoked;
            }

            if (storedToken.IsExpired)
            {
                return IAMResult
                    .Failure
                    .RefreshTokenInvalid;
            }

            ApplicationUser? user =
                await userManager.FindByIdAsync(
                    storedToken.UserId.ToString());

            if (user is null)
            {
                return IAMResult
                    .Failure
                    .RefreshTokenUserNotFound;
            }

            IList<string> roles =
                await userManager.GetRolesAsync(user);

            AccessTokenResult newAccessToken =
                tokenService.CreateAccessToken(
                    user,
                    roles);

            var newRefreshToken =
                tokenService.CreateRefreshToken();

            var newRefreshTokenHash =
                tokenService.HashRefreshToken(
                    newRefreshToken);

            DateTime now = DateTime.UtcNow;

            storedToken.RevokedAtUtc = now;
            storedToken.ReplacedByTokenHash =
                newRefreshTokenHash;

            var replacementToken =
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    TokenHash = newRefreshTokenHash,
                    CreatedAtUtc = now,
                    ExpiresAtUtc =
                        now.AddDays(
                            jwtOptions.Value.RefreshTokenDays)
                };

            dbContext
                .Set<RefreshToken>()
                .Add(replacementToken);

            try
            {
                await dbContext.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IAMResult
                    .Failure
                    .RefreshTokenRevoked;
            }

            Response response =
                IdentityMapper.ToAuthResponse<Response>(
                    newAccessToken,
                    newRefreshToken);

            return response;
        }
    }
}