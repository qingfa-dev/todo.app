using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.IAM;

public static class Register
{
    public record Request : EmailPasswordParameter;

    public record Response : RegisterResponse;

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .ValidEmail();

            RuleFor(x => x.Password)
                .ValidPassword();
        }
    }

    public static class Handler
    {
        public static async Task<Result<Response>> Handle(
            Request request,
            UserManager<ApplicationUser> userManager,
            IValidator<Request> validator,
            CancellationToken cancellationToken)
        {
            var validationResult =
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

            var existingUser =
                await userManager.FindByEmailAsync(
                    request.Email);

            if (existingUser is not null)
            {
                return IAMResult
                    .Failure
                    .UserAlreadyExists;
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email.Trim(),
                UserName = request.Email.Trim()
            };

            var identityResult =
                await userManager.CreateAsync(
                    user,
                    request.Password);

            if (!identityResult.Succeeded)
            {
                var errors =
                    identityResult.ToErrors();
                return errors;
            }

            var response =
                IdentityMapper.ToRegisterResponse<Response>(user);

            return response;
        }
    }
}