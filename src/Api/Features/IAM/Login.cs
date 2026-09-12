using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Features.IAM.Services;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.IAM;

public static class Login
{
    public record Request : EmailPasswordParameter;

    public record Response : AuthResponse;

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .ValidEmail();

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode(
                    IAMResult.Failure.PasswordRequired.Code)
                .WithMessage(
                    IAMResult.Failure.PasswordRequired.Description);
        }
    }

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService tokenService,
            IApplicationDbContext dbContext,
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

            var email = request.Email.Trim();

            ApplicationUser? user =
                await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return IAMResult
                    .Failure
                    .InvalidCredentials;
            }

            SignInResult signInResult =
                await signInManager.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                return IAMResult
                    .Failure
                    .AccountLockedOut;
            }

            if (!signInResult.Succeeded)
            {
                return IAMResult
                    .Failure
                    .InvalidCredentials;
            }

            IList<string> roles =
                await userManager.GetRolesAsync(user);

            AccessTokenResult accessToken =
                tokenService.CreateAccessToken(
                    user,
                    roles);

            var refreshToken =
                tokenService.CreateRefreshToken();

            var refreshTokenHash =
                tokenService.HashRefreshToken(
                    refreshToken);

            var refreshTokenEntity =
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    TokenHash = refreshTokenHash,
                    CreatedAtUtc = DateTime.UtcNow,
                    ExpiresAtUtc =
                        DateTime.UtcNow.AddDays(
                            jwtOptions.Value.RefreshTokenDays)
                };

            dbContext
                .Set<RefreshToken>()
                .Add(refreshTokenEntity);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            Response response =
                IdentityMapper.ToAuthResponse<Response>(
                    accessToken,
                    refreshToken);

            return response;
        }
    }
}