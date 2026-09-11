using Todo.Api.Domain.IAM.Constants;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Domain.IAM.Results;

public static class IAMResult
{
    public static class Failure
    {
        public static Error EmailRequired =>
            Error.BadRequest(
                "Identity.Email.Required",
                "Email is required.");

        public static Error EmailInvalid =>
            Error.BadRequest(
                "Identity.Email.Invalid",
                "Email address is invalid.");

        public static Error EmailTooLong =>
            Error.BadRequest(
                "Identity.Email.TooLong",
                $"Email cannot exceed " +
                $"{IAMConstant.Constraints.EmailMaxLength} characters.");

        public static Error PasswordRequired =>
            Error.BadRequest(
                "Identity.Password.Required",
                "Password is required.");

        public static Error PasswordTooShort =>
            Error.BadRequest(
                "Identity.Password.TooShort",
                $"Password must contain at least " +
                $"{IAMConstant.Constraints.PasswordMinLength} characters.");

        public static Error PasswordTooLong =>
            Error.BadRequest(
                "Identity.Password.TooLong",
                $"Password cannot exceed " +
                $"{IAMConstant.Constraints.PasswordMaxLength} characters.");

        public static Error PasswordRequiresDigit =>
            Error.BadRequest(
                "Identity.Password.RequiresDigit",
                "Password must contain at least one digit.");

        public static Error PasswordRequiresLowercase =>
            Error.BadRequest(
                "Identity.Password.RequiresLowercase",
                "Password must contain at least one lowercase letter.");

        public static Error PasswordRequiresUppercase =>
            Error.BadRequest(
                "Identity.Password.RequiresUppercase",
                "Password must contain at least one uppercase letter.");

        public static Error PasswordRequiresNonAlphanumeric =>
            Error.BadRequest(
                "Identity.Password.RequiresNonAlphanumeric",
                "Password must contain at least one non-alphanumeric character.");

        public static Error InvalidCredentials =>
            Error.Unauthorized(
                "Identity.InvalidCredentials",
                "Invalid email or password.");

        public static Error UserAlreadyExists =>
            Error.Conflict(
                "Identity.User.AlreadyExists",
                "A user with this email already exists.");

        public static Error UserCreationFailed =>
            Error.BadRequest(
                "Identity.User.CreationFailed",
                "The user could not be created.");

        public static Error UserNotFound =>
            Error.NotFound(
                "Identity.User.NotFound",
                "User was not found.");

        public static Error AccountLockedOut =>
            Error.Unauthorized(
                "Identity.Account.LockedOut",
                "The account is temporarily locked.");

        public static Error RefreshTokenRequired =>
            Error.BadRequest(
                "Identity.RefreshToken.Required",
                "Refresh token is required.");

        public static Error RefreshTokenInvalid =>
            Error.Unauthorized(
                "Identity.RefreshToken.Invalid",
                "Refresh token is invalid or expired.");

        public static Error RefreshTokenRevoked =>
            Error.Unauthorized(
                "Identity.RefreshToken.Revoked",
                "Refresh token has been revoked.");

        public static Error RefreshTokenUserNotFound =>
            Error.Unauthorized(
                "Identity.RefreshToken.UserNotFound",
                "The user associated with the refresh token was not found.");

        public static Error LogoutFailed =>
            Error.BadRequest(
                "Identity.Logout.Failed",
                "The refresh token could not be revoked.");
    }
}