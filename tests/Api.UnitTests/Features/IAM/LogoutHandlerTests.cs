using FluentValidation;
using Moq;
using Shouldly;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Features.IAM;
using Todo.Api.Features.IAM.Services;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Data;
using Todo.Api.UnitTests.Fixtures;

namespace Todo.Api.UnitTests.Features.IAM;

public class LogoutHandlerTests : IDisposable
{
    private readonly (ApplicationDbContext Context, IApplicationDbContext AppContext) _db;
    private readonly Mock<IJwtTokenService> _tokenServiceMock;
    private readonly IValidator<Logout.Request> _validator = new Logout.Validator();
    private readonly CancellationToken _ct = CancellationToken.None;

    public LogoutHandlerTests()
    {
        _db = TestDbContextFactory.Create();
        _tokenServiceMock = new Mock<IJwtTokenService>();
    }

    public void Dispose() => _db.Context.Dispose();

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailure()
    {
        var request = new Logout.Request
        {
            RefreshToken = ""
        };

        var result = await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ShouldReturnSuccess()
    {
        var request = new Logout.Request
        {
            RefreshToken = "nonexistent-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("nonexistent-token"))
            .Returns("nonexistent-hash");

        var result = await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenTokenFound_ShouldRevokeToken()
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

        var request = new Logout.Request
        {
            RefreshToken = "some-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("some-token"))
            .Returns(tokenHash);

        var result = await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();

        var updatedToken = _db.Context.Set<RefreshToken>().Find(refreshToken.Id);
        updatedToken.ShouldNotBeNull();
        updatedToken.RevokedAtUtc.ShouldNotBeNull();
    }

    [Fact]
    public async Task Handle_WhenTokenAlreadyRevoked_ShouldReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "token-hash";

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            RevokedAtUtc = DateTime.UtcNow.AddHours(-1)
        };

        _db.Context.Set<RefreshToken>().Add(refreshToken);
        _db.Context.SaveChanges();

        var request = new Logout.Request
        {
            RefreshToken = "some-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("some-token"))
            .Returns(tokenHash);

        var result = await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ShouldNotPersist()
    {
        var request = new Logout.Request
        {
            RefreshToken = "nonexistent-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("nonexistent-token"))
            .Returns("nonexistent-hash");

        var countBefore = _db.Context.Set<RefreshToken>().Count();

        await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        var countAfter = _db.Context.Set<RefreshToken>().Count();
        countAfter.ShouldBe(countBefore);
    }

    [Fact]
    public async Task Handle_ShouldCallHashRefreshToken()
    {
        var request = new Logout.Request
        {
            RefreshToken = "my-token"
        };

        _tokenServiceMock.Setup(x =>
            x.HashRefreshToken("my-token"))
            .Returns("my-hash");

        await Logout.Handler.Handle(
            request, _db.AppContext, _tokenServiceMock.Object, _validator, _ct);

        _tokenServiceMock.Verify(x =>
            x.HashRefreshToken("my-token"), Times.Once);
    }
}
