using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.IAM;

public class RegisterHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly IValidator<Register.Request> _validator = new Register.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public RegisterHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _userManagerMock = MockUserManager<ApplicationUser>();
    }

    public void Dispose() => _db.Context.Dispose();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new Register.Request
        {
            Email = "",
            Password = "ValidPass1!"
        };

        var result = await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldReturnUserAlreadyExists()
    {
        var request = new Register.Request
        {
            Email = "existing@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email
            });

        var result = await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.UserAlreadyExists.Code);
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_ShouldReturnFailure()
    {
        var request = new Register.Request
        {
            Email = "new@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Code = "TestError", Description = "Test error" }));

        var result = await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var request = new Register.Request
        {
            Email = "new@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Email.ShouldBe("new@example.com");
        result.Value.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldTrimEmail()
    {
        var request = new Register.Request
        {
            Email = "  new@example.com  ",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync("new@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Email.ShouldBe("new@example.com");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCreateUserWithCorrectProperties()
    {
        var request = new Register.Request
        {
            Email = "test@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        ApplicationUser? capturedUser = null;
        _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .Callback<ApplicationUser, string>((user, _) => capturedUser = user)
            .ReturnsAsync(IdentityResult.Success);

        await Register.Handler.Handle(
            request, _userManagerMock.Object, _validator, _ct);

        capturedUser.ShouldNotBeNull();
        capturedUser.Email.ShouldBe("test@example.com");
        capturedUser.UserName.ShouldBe("test@example.com");
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>()
        where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<TUser>>(),
            Array.Empty<IUserValidator<TUser>>(),
            Array.Empty<IPasswordValidator<TUser>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<TUser>>>());
    }
}
