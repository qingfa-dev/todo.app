using FluentValidation;
using FluentValidation.Results;

using Shouldly;

using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Authentications;

namespace Todo.Api.UnitTests.Features.IAM;

public class RegisterValidatorTests
{
    private readonly Register.Validator _validator = new();

    #region Email

    [Fact]
    public void Email_WhenEmpty_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = string.Empty,
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.EmailRequired.Code);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    public void Email_WhenInvalid_ShouldHaveError(string email)
    {
        var request = new Register.Request
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
    public void Email_WhenValid_ShouldNotHaveEmailError(string email)
    {
        var request = new Register.Request
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
        var request = new Register.Request
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
    public void Password_WhenTooShort_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "Ab1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordTooShort.Code);
    }

    [Fact]
    public void Password_WhenTooLong_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = new string('A', IAMConstant.Constraints.PasswordMaxLength + 1) + "1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordTooLong.Code);
    }

    [Fact]
    public void Password_WhenNoDigit_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "Password!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequiresDigit.Code);
    }

    [Fact]
    public void Password_WhenNoLowercase_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "PASSWORD1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequiresLowercase.Code);
    }

    [Fact]
    public void Password_WhenNoUppercase_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequiresUppercase.Code);
    }

    [Fact]
    public void Password_WhenNoNonAlphanumeric_ShouldHaveError()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "Password1"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorCode == IAMResult.Failure.PasswordRequiresNonAlphanumeric.Code);
    }

    [Fact]
    public void Password_WhenValid_ShouldNotHavePasswordErrors()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.Errors.ShouldNotContain(e =>
            e.PropertyName == nameof(Register.Request.Password));
    }

    #endregion

    #region Valid request

    [Fact]
    public void Validate_WhenValidRequest_ShouldBeValid()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
