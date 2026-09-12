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
using Todo.Api.Features.IAM.Services;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.IAM;

public class RefreshHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IJwtTokenService> _tokenServiceMock;
    private readonly IValidator<Refresh.Request> _validator = new Refresh.Validator();
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly CancellationToken _ct = CancellationToken.None;

    public RefreshHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _userManagerMock = MockUserManager<ApplicationUser>();
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
        var request = new Refresh.Request
        {
            RefreshToken = ""
        };

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ShouldReturnRefreshTokenInvalid()
    {
        var request = new Refresh.Request
        {
            RefreshToken = "nonexistent-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("nonexistent-token"))
            .Returns("nonexistent-hash");

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.RefreshTokenInvalid.Code);
    }

    [Fact]
    public async Task Handle_WhenTokenRevoked_ShouldReturnRefreshTokenRevoked()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(5),
            RevokedAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "revoked-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("revoked-token"))
            .Returns(tokenHash);

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.RefreshTokenRevoked.Code);
    }

    [Fact]
    public async Task Handle_WhenTokenExpired_ShouldReturnRefreshTokenInvalid()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-10),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "expired-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("expired-token"))
            .Returns(tokenHash);

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.RefreshTokenInvalid.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnRefreshTokenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "valid-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("valid-token"))
            .Returns(tokenHash);

        _userManagerMock.Setup(x =>
            x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.Code == IAMResult.Failure.RefreshTokenUserNotFound.Code);
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

        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "valid-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("valid-token"))
            .Returns(tokenHash);

        _userManagerMock.Setup(x =>
            x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("new-refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("new-refresh-token"))
            .Returns("new-refresh-token-hash");

        var result = await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldBe("new-access-token");
        result.Value.RefreshToken.ShouldBe("new-refresh-token");
        result.Value.TokenType.ShouldBe("Bearer");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldRevokeOldToken()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "valid-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("valid-token"))
            .Returns(tokenHash);

        _userManagerMock.Setup(x =>
            x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("new-refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("new-refresh-token"))
            .Returns("new-refresh-token-hash");

        await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        var updatedToken = _db.Context.Set<RefreshToken>().Find(refreshToken.Id);
        updatedToken.ShouldNotBeNull();
        updatedToken.RevokedAtUtc.ShouldNotBeNull();
        updatedToken.ReplacedByTokenHash.ShouldBe("new-refresh-token-hash");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCreateReplacementToken()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "valid-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("valid-token"))
            .Returns(tokenHash);

        _userManagerMock.Setup(x =>
            x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("new-refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("new-refresh-token"))
            .Returns("new-refresh-token-hash");

        await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        var allTokens = _db.Context.Set<RefreshToken>().ToList();
        allTokens.Count.ShouldBe(2);

        var newToken = allTokens.First(x => x.TokenHash == "new-refresh-token-hash");
        newToken.UserId.ShouldBe(userId);
        newToken.RevokedAtUtc.ShouldBeNull();
        newToken.ExpiresAtUtc.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCallTokenService()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "user@example.com",
            UserName = "user@example.com"
        };

        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Refresh.Request
        {
            RefreshToken = "valid-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("valid-token"))
            .Returns(tokenHash);

        _userManagerMock.Setup(x =>
            x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x =>
            x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        _tokenServiceMock.Setup(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(new AccessTokenResult("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        _tokenServiceMock.Setup(x =>
            x.CreateRefreshToken())
            .Returns("new-refresh-token");

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("new-refresh-token"))
            .Returns("new-refresh-token-hash");

        await Refresh.Handler.Handle(
            request,
            _db.AppContext,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _jwtOptions,
            _validator,
            _ct);

        _tokenServiceMock.Verify(x =>
            x.HashRefreshToken("valid-token"), Times.Once);
        _tokenServiceMock.Verify(x =>
            x.CreateAccessToken(user, It.IsAny<IList<string>>()), Times.Once);
        _tokenServiceMock.Verify(x =>
            x.CreateRefreshToken(), Times.Once);
        _tokenServiceMock.Verify(x =>
            x.HashRefreshToken("new-refresh-token"), Times.Once);
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
