using FluentValidation;
using FluentValidation.Results;

using Shouldly;

using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Authentications;

namespace Todo.Api.UnitTests.Features.IAM;

public class LoginValidatorTests
{
    private readonly Login.Validator _validator = new();

    #region Email

    [Fact]
    public void Email_WhenEmpty_ShouldHaveError()
    {
        var request = new Login.Request
        {
            Email = string.Empty,
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailRequired.Code);
    }

    [Fact]
    public void Email_WhenNull_ShouldHaveError()
    {
        var request = new Login.Request
        {
            Email = null!,
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailRequired.Code);
    }

    [Fact]
    public void Email_WhenExceedsMaxLength_ShouldHaveError()
    {
        var request = new Login.Request
        {
            Email = new string('a', IAMConstant.Constraints.EmailMaxLength + 1) + "@test.com",
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailTooLong.Code);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("@test.com")]
    public void Email_WhenInvalid_ShouldHaveError(string email)
    {
        var request = new Login.Request
        {
            Email = email,
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailInvalid.Code);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co")]
    public void Email_WhenValid_ShouldNotHaveError(string email)
    {
        var request = new Login.Request
        {
            Email = email,
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailRequired.Code);
        result.Errors.ShouldNotContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailInvalid.Code);
    }

    #endregion

    #region Password

    [Fact]
    public void Password_WhenEmpty_ShouldHaveError()
    {
        var request = new Login.Request
        {
            Email = "test@example.com",
            Password = string.Empty
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequired.Code);
    }

    [Fact]
    public void Password_WhenNull_ShouldHaveError()
    {
        var request = new Login.Request
        {
            Email = "test@example.com",
            Password = null!
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequired.Code);
    }

    #endregion

    #region Valid request

    [Fact]
    public void Validate_WhenValidRequest_ShouldBeValid()
    {
        var request = new Login.Request
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
