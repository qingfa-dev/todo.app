using Microsoft.AspNetCore.Identity;

using Shouldly;

using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Extensions;

public class IdentityResultExtensionsTests
{
    [Fact]
    public void ToErrors_WhenSuccess_ShouldReturnEmptyArray()
    {
        IdentityResult identityResult = IdentityResult.Success;

        Error[] errors = identityResult.ToErrors();

        errors.ShouldBeEmpty();
    }

    [Fact]
    public void ToErrors_WhenDuplicateEmail_ShouldReturnConflictError()
    {
        var identityResult = IdentityResult.Failed(
            new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email already exists."
            });

        Error[] errors = identityResult.ToErrors();

        errors.Length.ShouldBe(1);
        errors[0].Code.ShouldBe("DuplicateEmail");
        errors[0].Description.ShouldBe("Email already exists.");
        errors[0].StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public void ToErrors_WhenDuplicateUserName_ShouldReturnConflictError()
    {
        var identityResult = IdentityResult.Failed(
            new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Username already exists."
            });

        Error[] errors = identityResult.ToErrors();

        errors.Length.ShouldBe(1);
        errors[0].Code.ShouldBe("DuplicateUserName");
        errors[0].StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public void ToErrors_WhenOtherError_ShouldReturnBadRequestError()
    {
        var identityResult = IdentityResult.Failed(
            new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password is too short."
            });

        Error[] errors = identityResult.ToErrors();

        errors.Length.ShouldBe(1);
        errors[0].Code.ShouldBe("PasswordTooShort");
        errors[0].Description.ShouldBe("Password is too short.");
        errors[0].StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ToErrors_WhenMultipleErrors_ShouldMapAll()
    {
        var identityResult = IdentityResult.Failed(
            new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email already exists."
            },
            new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password is too short."
            });

        Error[] errors = identityResult.ToErrors();

        errors.Length.ShouldBe(2);
    }
}
