using FluentValidation.Results;

using Shouldly;

using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.UnitTests.SharedKernel.Extensions;

public class ValidationResultExtensionsTests
{
    [Fact]
    public void ToErrors_WhenEmpty_ShouldReturnEmptyArray()
    {
        var validationResult = new ValidationResult();

        Error[] errors = validationResult.ToErrors();

        errors.ShouldBeEmpty();
    }

    [Fact]
    public void ToErrors_WhenHasErrors_ShouldMapToErrorArray()
    {
        var validationResult = new ValidationResult(
        [
            new ValidationFailure("Name", "Name is required")
            {
                ErrorCode = "Name.Required"
            },
            new ValidationFailure("Email", "Email is invalid")
            {
                ErrorCode = "Email.Invalid"
            }
        ]);

        Error[] errors = validationResult.ToErrors();

        errors.Length.ShouldBe(2);
        errors[0].Code.ShouldBe("Name.Required");
        errors[0].Description.ShouldBe("Name is required");
        errors[0].StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        errors[1].Code.ShouldBe("Email.Invalid");
        errors[1].Description.ShouldBe("Email is invalid");
    }

    [Fact]
    public void ToErrors_ShouldOnlyMapToBadRequestErrors()
    {
        var validationResult = new ValidationResult(
        [
            new ValidationFailure("Field", "Error")
            {
                ErrorCode = "Test.Code"
            }
        ]);

        Error[] errors = validationResult.ToErrors();

        errors[0].StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
}
