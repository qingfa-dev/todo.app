using FluentValidation;

using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Results;

namespace Todo.Api.Features.IAM;

public static class IdentityValidationExtensions
{
    public static IRuleBuilderOptions<T, string>
        ValidEmail<T>(
            this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(
                IAMResult.Failure.EmailRequired.Code)
            .WithMessage(
                IAMResult.Failure.EmailRequired.Description)
            .MaximumLength(
                IAMConstant.Constraints.EmailMaxLength)
            .WithErrorCode(
                IAMResult.Failure.EmailTooLong.Code)
            .WithMessage(
                IAMResult.Failure.EmailTooLong.Description)
            .EmailAddress()
            .WithErrorCode(
                IAMResult.Failure.EmailInvalid.Code)
            .WithMessage(
                IAMResult.Failure.EmailInvalid.Description);
    }

    public static IRuleBuilderOptions<T, string>
        ValidPassword<T>(
            this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(
                IAMResult.Failure.PasswordRequired.Code)
            .WithMessage(
                IAMResult.Failure.PasswordRequired.Description)
            .MinimumLength(
                IAMConstant.Constraints.PasswordMinLength)
            .WithErrorCode(
                IAMResult.Failure.PasswordTooShort.Code)
            .WithMessage(
                IAMResult.Failure.PasswordTooShort.Description)
            .MaximumLength(
                IAMConstant.Constraints.PasswordMaxLength)
            .WithErrorCode(
                IAMResult.Failure.PasswordTooLong.Code)
            .WithMessage(
                IAMResult.Failure.PasswordTooLong.Description)
            .Matches("[0-9]")
            .WithErrorCode(
                IAMResult.Failure.PasswordRequiresDigit.Code)
            .WithMessage(
                IAMResult.Failure.PasswordRequiresDigit.Description)
            .Matches("[a-z]")
            .WithErrorCode(
                IAMResult.Failure.PasswordRequiresLowercase.Code)
            .WithMessage(
                IAMResult.Failure.PasswordRequiresLowercase.Description)
            .Matches("[A-Z]")
            .WithErrorCode(
                IAMResult.Failure.PasswordRequiresUppercase.Code)
            .WithMessage(
                IAMResult.Failure.PasswordRequiresUppercase.Description)
            .Matches(@"[^a-zA-Z0-9]")
            .WithErrorCode(
                IAMResult.Failure.PasswordRequiresNonAlphanumeric.Code)
            .WithMessage(
                IAMResult.Failure.PasswordRequiresNonAlphanumeric.Description);
    }
}