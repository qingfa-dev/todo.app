using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Domain.IAM.Results;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Authentications;
using Todo.Api.Features.IAM.Services;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.IAM;

public class LoginHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IJwtTokenService> _tokenServiceMock;
    private readonly IValidator<Login.Request> _validator = new Login.Validator();
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly CancellationToken _ct = CancellationToken.None;

    public LoginHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _userManagerMock = MockUserManager<ApplicationUser>();
        _signInManagerMock = MockSignInManager(_userManagerMock);
        _tokenServiceMock = new Mock<IJwtTokenService>();
        _jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "SuperSecretKey12345678901234567890",
            AccessTokenMinutes = 15,
            RefreshTokenDays = 7
        });
    }

    public void Dispose() => _db.Context.Dispose();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new Login.Request
        {
            Email = "",
            Password = ""
        };

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnInvalidCredentials()
    {
        var request = new Login.Request
        {
            Email = "nonexistent@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.InvalidCredentials.Code);
    }

    [Fact]
    public async Task Handle_WhenAccountLockedOut_ShouldReturnAccountLockedOut()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "locked@example.com",
            UserName = "locked@example.com"
        };

        var request = new Login.Request
        {
            Email = "locked@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.LockedOut);

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.AccountLockedOut.Code);
    }

    [Fact]
    public async Task Handle_WhenWrongPassword_ShouldReturnInvalidCredentials()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var request = new Login.Request
        {
            Email = "user@example.com",
            Password = "WrongPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.Failed);

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.InvalidCredentials.Code);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var request = new Login.Request
        {
            Email = "user@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("refresh-token"))
            .Returns("refresh-token-hash");

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldBe("access-token");
        result.Value.RefreshToken.ShouldBe("refresh-token");
        result.Value.TokenType.ShouldBe("Bearer");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldPersistRefreshToken()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var request = new Login.Request
        {
            Email = "user@example.com",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("refresh-token"))
            .Returns("refresh-token-hash");

        await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        var storedToken = _db.Context.Set<RefreshToken>()
            .FirstOrDefault(x => x.TokenHash == "refresh-token-hash");

        storedToken.ShouldNotBeNull();
        storedToken.UserId.ShouldBe(user.Id);
        storedToken.IsExpired.ShouldBeFalse();
        storedToken.IsRevoked.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldTrimEmail()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var request = new Login.Request
        {
            Email = "  user@example.com  ",
            Password = "ValidPass1!"
        };

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync("user@example.com"))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("refresh-token"))
            .Returns("refresh-token-hash");

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnCorrectExpiresIn()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var request = new Login.Request
        {
            Email = "user@example.com",
            Password = "ValidPass1!"
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(30);

        _userManagerMock.Setup(x =>
            x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("access-token", expiresAt));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("refresh-token"))
            .Returns("refresh-token-hash");

        var result = await Login.Handler.Handle(
            request,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object,
            _db.AppContext,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ExpiresIn.ShouldBeGreaterThanOrEqualTo(0);
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

    private static Mock<SignInManager<TUser>> MockSignInManager<TUser>(
        Mock<UserManager<TUser>> userManagerMock)
        where TUser : class
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        var options = Mock.Of<IOptions<IdentityOptions>>();
        var logger = Mock.Of<ILogger<SignInManager<TUser>>>();
        var schemes = Mock.Of<IAuthenticationSchemeProvider>();

        var confirmation = Mock.Of<IUserConfirmation<TUser>>();

        return new Mock<SignInManager<TUser>>(
            userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options,
            logger,
            schemes,
            confirmation);
    }
}
